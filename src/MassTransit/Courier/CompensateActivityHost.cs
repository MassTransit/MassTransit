namespace MassTransit.Courier
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Contracts;
    using Logging;
    using Util;


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
                    activity?.RecordException(ex, escaped: false);

                    await compensateContext.Failed(ex).Evaluate().ConfigureAwait(false);
                }

                await context.NotifyConsumed(timer.Elapsed, TypeCache<TActivity>.ShortName).ConfigureAwait(false);

                await next.Send(context).ConfigureAwait(false);
            }
            catch (OperationCanceledException exception)
            {
                await ThrowHelper.ConsumeFilter.ThrowOperationCancelled<TActivity, RoutingSlip>(context, timer.Elapsed, exception, activity)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                activity?.RecordException(ex, escaped: true);

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
