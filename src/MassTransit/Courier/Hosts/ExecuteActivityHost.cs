namespace MassTransit.Courier.Hosts
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Context;
    using Contracts;
    using GreenPipes;
    using Logging;
    using Util;


    public class ExecuteActivityHost<TActivity, TArguments> :
        IFilter<ConsumeContext<RoutingSlip>>
        where TActivity : class, IExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IExecuteActivityFactory<TActivity, TArguments> _activityFactory;
        readonly Uri _compensateAddress;
        readonly IRequestPipe<ExecuteActivityContext<TActivity, TArguments>, ExecutionResult> _executePipe;

        public ExecuteActivityHost(IExecuteActivityFactory<TActivity, TArguments> activityFactory, IPipe<RequestContext> executePipe, Uri compensateAddress)
            : this(activityFactory, executePipe)
        {
            if (compensateAddress == null)
                throw new ArgumentNullException(nameof(compensateAddress));

            _compensateAddress = compensateAddress;
        }

        public ExecuteActivityHost(IExecuteActivityFactory<TActivity, TArguments> activityFactory, IPipe<RequestContext> executePipe)
        {
            if (activityFactory == null)
                throw new ArgumentNullException(nameof(activityFactory));

            _activityFactory = activityFactory;

            _executePipe = executePipe.CreateRequestPipe<ExecuteActivityContext<TActivity, TArguments>, ExecutionResult>();
        }

        public async Task Send(ConsumeContext<RoutingSlip> context, IPipe<ConsumeContext<RoutingSlip>> next)
        {
            var activity = LogContext.IfEnabled(OperationName.Courier.Compensate)?.StartActivity(new
            {
                ActivityType = TypeMetadataCache<TActivity>.ShortName,
                ArgumentType = TypeMetadataCache<TArguments>.ShortName
            });

            var timer = Stopwatch.StartNew();
            try
            {
                await Task.Yield();

                ExecuteContext<TArguments> executeContext = new HostExecuteContext<TArguments>(HostMetadataCache.Host, _compensateAddress, context);

                LogContext.Debug?.Log("Execute Activity: {TrackingNumber} ({Activity}, {Host})", executeContext.TrackingNumber,
                    TypeMetadataCache<TActivity>.ShortName, context.ReceiveContext.InputAddress);

                try
                {
                    var result = await _activityFactory.Execute(executeContext, _executePipe).Result().ConfigureAwait(false);

                    await result.Evaluate().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    var result = executeContext.Faulted(ex);

                    await result.Evaluate().ConfigureAwait(false);
                }

                await context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<TActivity>.ShortName).ConfigureAwait(false);

                await next.Send(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await context.NotifyFaulted(timer.Elapsed, TypeMetadataCache<TActivity>.ShortName, ex).ConfigureAwait(false);

                LogContext.Error?.Log(ex, "Activity {Activity} execution faulted: {Exception}", TypeMetadataCache<TActivity>.ShortName);

                throw;
            }
            finally
            {
                activity?.Stop();
            }
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("executeActivity");
            scope.Set(new
            {
                ActivityType = TypeMetadataCache<TActivity>.ShortName,
                ArgumentType = TypeMetadataCache<TArguments>.ShortName
            });
            if (_compensateAddress != null)
                scope.Add("compensateAddress", _compensateAddress);

            _executePipe.Probe(scope);
        }
    }
}
