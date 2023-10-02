namespace MassTransit.Futures
{
    using System.Threading.Tasks;
    using Courier.Contracts;


    public class PlanRoutingSlipExecutor<TInput> :
        IRoutingSlipExecutor<TInput>
        where TInput : class
    {
        public async Task Execute(BehaviorContext<FutureState, TInput> context)
        {
            var itineraryPlanner = context.GetServiceOrCreateInstance<IItineraryPlanner<TInput>>();

            var trackingNumber = NewId.NextGuid();

            var builder = new RoutingSlipBuilder(trackingNumber);

            builder.AddVariable(MessageHeaders.FutureId, context.CorrelationId);

            builder.AddSubscription(context.ReceiveContext.InputAddress, RoutingSlipEvents.Completed | RoutingSlipEvents.Faulted);

            await itineraryPlanner.PlanItinerary(context, builder).ConfigureAwait(false);

            var routingSlip = builder.Build();

            await context.Execute(routingSlip, context.CancellationToken).ConfigureAwait(false);

            if (TrackRoutingSlip)
                context.Saga.Pending.Add(trackingNumber);
        }

        public bool TrackRoutingSlip { get; set; }
    }
}
