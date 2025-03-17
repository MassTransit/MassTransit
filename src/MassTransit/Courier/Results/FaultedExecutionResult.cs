namespace MassTransit.Courier.Results
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Events;
    using Messages;


    class FaultedExecutionResult<TArguments> :
        BaseExecutionResult<TArguments>,
        FaultedActivityOptions
        where TArguments : class
    {
        readonly ActivityException _activityException;
        readonly TimeSpan _elapsed;
        readonly Exception _exception;
        readonly ExceptionInfo _exceptionInfo;

        public FaultedExecutionResult(ExecuteContext<TArguments> context, IRoutingSlipEventPublisher publisher, Activity activity, RoutingSlip routingSlip,
            Exception exception)
            : base(context, publisher, activity, routingSlip)
        {
            _exception = exception;
            _exceptionInfo = new FaultExceptionInfo(exception);
            _elapsed = Context.Elapsed;

            _activityException = new RoutingSlipActivityException(Activity.Name, Context.Host, Context.ExecutionId,
                Context.Timestamp, _elapsed, _exceptionInfo);
        }

        public override async Task Evaluate()
        {
            var builder = CreateRoutingSlipBuilder(RoutingSlip);

            Build(builder);

            var routingSlip = builder.Build();

            await Publisher.PublishRoutingSlipActivityFaulted(Context.ActivityName, Context.ExecutionId, Context.Timestamp,
                _elapsed, _exceptionInfo, routingSlip.Variables, Activity.Arguments).ConfigureAwait(false);

            if (HasCompensationLogs(routingSlip))
                await Context.Forward(routingSlip.GetNextCompensateAddress(), routingSlip).ConfigureAwait(false);
            else
            {
                var faultedTimestamp = Context.Timestamp + _elapsed;
                var faultedDuration = faultedTimestamp - routingSlip.CreateTimestamp;

                await Publisher.PublishRoutingSlipFaulted(faultedTimestamp, faultedDuration, routingSlip.Variables, _activityException).ConfigureAwait(false);
            }
        }

        public override bool IsFaulted(out Exception exception)
        {
            exception = _exception;
            return true;
        }

        static bool HasCompensationLogs(RoutingSlip routingSlip)
        {
            return routingSlip.CompensateLogs is { Count: > 0 };
        }

        protected virtual void Build(RoutingSlipBuilder builder)
        {
            builder.AddActivityException(_activityException);

            if (Variables?.Any() ?? false)
                builder.SetVariables(Variables);
        }

        static RoutingSlipBuilder CreateRoutingSlipBuilder(RoutingSlip routingSlip)
        {
            return new RoutingSlipBuilder(routingSlip, routingSlip.Itinerary, []);
        }
    }
}
