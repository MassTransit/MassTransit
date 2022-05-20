namespace MassTransit.Courier.Results
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Events;
    using Messages;


    class FaultedExecutionResult<TArguments> :
        ExecutionResult
        where TArguments : class
    {
        readonly Activity _activity;
        readonly ActivityException _activityException;
        readonly TimeSpan _elapsed;
        readonly Exception _exception;
        readonly ExceptionInfo _exceptionInfo;
        readonly ExecuteContext<TArguments> _executeContext;
        readonly IRoutingSlipEventPublisher _publisher;
        readonly RoutingSlip _routingSlip;

        public FaultedExecutionResult(ExecuteContext<TArguments> executeContext, IRoutingSlipEventPublisher publisher, Activity activity,
            RoutingSlip routingSlip, Exception exception)
        {
            _executeContext = executeContext;
            _publisher = publisher;
            _activity = activity;
            _routingSlip = routingSlip;
            _exception = exception;
            _exceptionInfo = new FaultExceptionInfo(exception);
            _elapsed = _executeContext.Elapsed;

            _activityException = new RoutingSlipActivityException(_activity.Name, _executeContext.Host, _executeContext.ExecutionId,
                _executeContext.Timestamp, _elapsed, _exceptionInfo);
        }

        public async Task Evaluate()
        {
            var builder = CreateRoutingSlipBuilder(_routingSlip);

            Build(builder);

            var routingSlip = builder.Build();

            await _publisher.PublishRoutingSlipActivityFaulted(_executeContext.ActivityName, _executeContext.ExecutionId, _executeContext.Timestamp,
                _elapsed, _exceptionInfo, routingSlip.Variables, _activity.Arguments).ConfigureAwait(false);

            if (HasCompensationLogs(routingSlip))
                await _executeContext.Forward(routingSlip.GetNextCompensateAddress(), routingSlip).ConfigureAwait(false);
            else
            {
                var faultedTimestamp = _executeContext.Timestamp + _elapsed;
                var faultedDuration = faultedTimestamp - routingSlip.CreateTimestamp;

                await _publisher.PublishRoutingSlipFaulted(faultedTimestamp, faultedDuration, routingSlip.Variables, _activityException).ConfigureAwait(false);
            }
        }

        public virtual bool IsFaulted(out Exception exception)
        {
            exception = _exception;
            return true;
        }

        static bool HasCompensationLogs(RoutingSlip routingSlip)
        {
            return routingSlip.CompensateLogs != null && routingSlip.CompensateLogs.Count > 0;
        }

        protected virtual void Build(RoutingSlipBuilder builder)
        {
            builder.AddActivityException(_activityException);
        }

        static RoutingSlipBuilder CreateRoutingSlipBuilder(RoutingSlip routingSlip)
        {
            return new RoutingSlipBuilder(routingSlip, routingSlip.Itinerary, Enumerable.Empty<Activity>());
        }
    }
}
