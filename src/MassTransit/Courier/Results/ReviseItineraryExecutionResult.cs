namespace MassTransit.Courier.Results
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;


    class ReviseItineraryExecutionResult<TArguments> :
        CompletedExecutionResult<TArguments>
        where TArguments : class
    {
        readonly Action<IItineraryBuilder> _itineraryBuilder;

        public ReviseItineraryExecutionResult(ExecuteContext<TArguments> context, IRoutingSlipEventPublisher publisher, Activity activity,
            RoutingSlip routingSlip, Uri compensationAddress, Action<IItineraryBuilder> itineraryBuilder)
            : base(context, publisher, activity, routingSlip, compensationAddress)
        {
            _itineraryBuilder = itineraryBuilder;
        }

        protected override void Build(RoutingSlipBuilder builder)
        {
            base.Build(builder);

            _itineraryBuilder(builder);
        }

        protected override RoutingSlipBuilder CreateRoutingSlipBuilder(RoutingSlip routingSlip)
        {
            return new RoutingSlipBuilder(routingSlip, [], routingSlip.Itinerary.Skip(1));
        }

        protected override async Task PublishActivityEvents(RoutingSlip routingSlip, RoutingSlipBuilder builder)
        {
            await base.PublishActivityEvents(routingSlip, builder).ConfigureAwait(false);

            await Publisher.PublishRoutingSlipRevised(Context.ActivityName, Context.ExecutionId, Context.Timestamp, Context.Elapsed, routingSlip.Variables,
                routingSlip.Itinerary, builder.SourceItinerary).ConfigureAwait(false);
        }
    }
}
