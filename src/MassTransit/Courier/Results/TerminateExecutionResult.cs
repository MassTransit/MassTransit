namespace MassTransit.Courier.Results
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;


    class TerminateExecutionResult<TArguments> :
        CompletedExecutionResult<TArguments>
        where TArguments : class
    {
        public TerminateExecutionResult(ExecuteContext<TArguments> context, IRoutingSlipEventPublisher publisher, Activity activity, RoutingSlip routingSlip,
            Uri compensationAddress)
            : base(context, publisher, activity, routingSlip, compensationAddress)
        {
        }

        protected override RoutingSlipBuilder CreateRoutingSlipBuilder(RoutingSlip routingSlip)
        {
            return new RoutingSlipBuilder(routingSlip, [], routingSlip.Itinerary.Skip(1));
        }

        protected override async Task PublishActivityEvents(RoutingSlip routingSlip, RoutingSlipBuilder builder)
        {
            await base.PublishActivityEvents(routingSlip, builder).ConfigureAwait(false);

            await Publisher.PublishRoutingSlipTerminated(Context.ActivityName, Context.ExecutionId, Context.Timestamp, Context.Elapsed, routingSlip.Variables,
                builder.SourceItinerary).ConfigureAwait(false);
        }
    }
}
