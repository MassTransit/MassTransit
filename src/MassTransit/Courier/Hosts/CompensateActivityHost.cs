namespace MassTransit.Courier.Hosts
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Context;
    using Contracts;
    using GreenPipes;
    using Logging;
    using Metadata;
    using Util;


    public class CompensateActivityHost<TActivity, TLog> :
        IFilter<ConsumeContext<RoutingSlip>>
        where TActivity : class, ICompensateActivity<TLog>
        where TLog : class
    {
        readonly ICompensateActivityFactory<TActivity, TLog> _activityFactory;
        readonly IRequestPipe<CompensateActivityContext<TActivity, TLog>, CompensationResult> _compensatePipe;

        public CompensateActivityHost(ICompensateActivityFactory<TActivity, TLog> activityFactory, IPipe<RequestContext> compensatePipe)
        {
            _activityFactory = activityFactory;

            _compensatePipe = compensatePipe.CreateRequestPipe<CompensateActivityContext<TActivity, TLog>, CompensationResult>();
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
                await Task.Yield();

                CompensateContext<TLog> compensateContext = new HostCompensateContext<TLog>(HostMetadataCache.Host, context);

                LogContext.Debug?.Log("Compensate Activity: {TrackingNumber} ({Activity}, {Host})", compensateContext.TrackingNumber,
                    TypeMetadataCache<TActivity>.ShortName, context.ReceiveContext.InputAddress);

                try
                {
                    var result = await _activityFactory.Compensate(compensateContext, _compensatePipe).Result().ConfigureAwait(false);

                    await result.Evaluate().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    var result = compensateContext.Failed(ex);

                    await result.Evaluate().ConfigureAwait(false);
                }

                await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<TActivity>.ShortName).ConfigureAwait(false);

                await next.Send(context).ConfigureAwait(false);
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
