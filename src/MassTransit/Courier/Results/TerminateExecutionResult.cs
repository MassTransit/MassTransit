namespace MassTransit.Courier.Results
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;


    class TerminateExecutionResult<TArguments> :
        CompletedExecutionResult<TArguments>
        where TArguments : class
    {
        public TerminateExecutionResult(ExecuteContext<TArguments> context, IRoutingSlipEventPublisher publisher, Activity activity, RoutingSlip routingSlip)
            : base(context, publisher, activity, routingSlip)
        {
        }

        protected override RoutingSlipBuilder CreateRoutingSlipBuilder(RoutingSlip routingSlip)
        {
            return new RoutingSlipBuilder(routingSlip, Enumerable.Empty<Activity>(), routingSlip.Itinerary.Skip(1));
        }

        protected override async Task PublishActivityEvents(RoutingSlip routingSlip, RoutingSlipBuilder builder)
        {
            await base.PublishActivityEvents(routingSlip, builder).ConfigureAwait(false);

            await Publisher.PublishRoutingSlipTerminated(Context.ActivityName, Context.ExecutionId, Context.Timestamp, Context.Elapsed, routingSlip.Variables,
                builder.SourceItinerary).ConfigureAwait(false);
        }
    }


    class TerminateWithVariablesExecutionResult<TArguments> :
        TerminateExecutionResult<TArguments>
        where TArguments : class
    {
        readonly IDictionary<string, object> _variables;

        public TerminateWithVariablesExecutionResult(ExecuteContext<TArguments> context, IRoutingSlipEventPublisher publisher, Activity activity,
            RoutingSlip routingSlip, IDictionary<string, object> variables)
            : base(context, publisher, activity, routingSlip)
        {
            _variables = variables;
        }

        protected override void Build(RoutingSlipBuilder builder)
        {
            base.Build(builder);

            builder.SetVariables(_variables);
        }
    }
}
