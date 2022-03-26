namespace MassTransit.Courier
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Contracts;
    using Logging;


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
            StartedActivity? activity = LogContext.Current?.StartCompensateActivity<TActivity, TLog>(context);

            var timer = Stopwatch.StartNew();
            try
            {
                CompensateContext<TLog> compensateContext = new HostCompensateContext<TLog>(context);

                LogContext.Debug?.Log("Compensate Activity: {TrackingNumber} ({Activity}, {Host})", compensateContext.TrackingNumber,
                    TypeCache<TActivity>.ShortName, context.ReceiveContext.InputAddress);

                try
                {
                    await _compensatePipe.Send(compensateContext).ConfigureAwait(false);

                    var result = compensateContext.Result
                        ?? compensateContext.Failed(new ActivityCompensationException("The activity compensation did not return a result"));

                    await result.Evaluate().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await compensateContext.Failed(ex).Evaluate().ConfigureAwait(false);
                }

                await context.NotifyConsumed(timer.Elapsed, TypeCache<TActivity>.ShortName).ConfigureAwait(false);

                await next.Send(context).ConfigureAwait(false);
            }
            catch (OperationCanceledException exception)
            {
                await context.NotifyFaulted(timer.Elapsed, TypeCache<TActivity>.ShortName, exception).ConfigureAwait(false);

                if (exception.CancellationToken == context.CancellationToken)
                    throw;

                throw new ConsumerCanceledException($"The operation was canceled by the activity: {TypeCache<TActivity>.ShortName}");
            }
            catch (Exception ex)
            {
                await context.NotifyFaulted(timer.Elapsed, TypeCache<TActivity>.ShortName, ex).ConfigureAwait(false);
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
                ActivityType = TypeCache<TActivity>.ShortName,
                LogType = TypeCache<TLog>.ShortName
            });

            _compensatePipe.Probe(scope);
        }
    }
}
