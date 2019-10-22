namespace MassTransit.Courier.Hosts
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using Contracts;
    using Exceptions;
    using GreenPipes;
    using Logging;
    using Metadata;


    public class CompensateActivityHost<TActivity, TLog> :
        IFilter<ConsumeContext<RoutingSlip>>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        readonly IPipe<CompensateContext<TLog>> _compensatePipe;

        public CompensateActivityHost(IPipe<CompensateContext<TLog>> compensatePipe)
        {
            _compensatePipe = compensatePipe;
        }

        public async Task Send(ConsumeContext<RoutingSlip> context, IPipe<ConsumeContext<RoutingSlip>> next)
        {
            var activity = LogContext.IfEnabled(OperationName.Courier.Compensate)?.StartActivity(new
            {
                ActivityType = TypeMetadataCache<TActivity>.ShortName,
                LogType = TypeMetadataCache<TLog>.ShortName
            });

            activity?.AddBaggage(DiagnosticHeaders.TrackingNumber, context.Message.TrackingNumber);

            var timer = Stopwatch.StartNew();
            try
            {
                CompensateContext<TLog> compensateContext = new HostCompensateContext<TLog>(context);

                LogContext.Debug?.Log("Compensate Activity: {TrackingNumber} ({Activity}, {Host})", compensateContext.TrackingNumber,
                    TypeMetadataCache<TActivity>.ShortName, context.ReceiveContext.InputAddress);

                try
                {
                    await Task.Yield();
                    await _compensatePipe.Send(compensateContext).ConfigureAwait(false);

                    var result = compensateContext.Result
                        ?? compensateContext.Failed(new ActivityCompensationException("The activity compensation did not return a result"));

                    await result.Evaluate().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await compensateContext.Failed(ex).Evaluate().ConfigureAwait(false);
                }

                await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<TActivity>.ShortName).ConfigureAwait(false);

                await next.Send(context).ConfigureAwait(false);
            }
            catch (OperationCanceledException exception)
            {
                await context.NotifyFaulted(timer.Elapsed, TypeMetadataCache<TActivity>.ShortName, exception).ConfigureAwait(false);

                if (exception.CancellationToken == context.CancellationToken)
                    throw;

                throw new ConsumerCanceledException($"The operation was cancelled by the activity: {TypeMetadataCache<TActivity>.ShortName}");
            }
            catch (Exception ex)
            {
                await context.NotifyFaulted(timer.Elapsed, TypeMetadataCache<TActivity>.ShortName, ex).ConfigureAwait(false);

                LogContext.Error?.Log(ex, "Activity {Activity} compensation faulted", TypeMetadataCache<TActivity>.ShortName);

                throw;
            }
            finally
            {
                activity?.Stop();
            }
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("compensateActivity");
            scope.Set(new
            {
                ActivityType = TypeMetadataCache<TActivity>.ShortName,
                LogType = TypeMetadataCache<TLog>.ShortName
            });

            _compensatePipe.Probe(scope);
        }
    }
}
