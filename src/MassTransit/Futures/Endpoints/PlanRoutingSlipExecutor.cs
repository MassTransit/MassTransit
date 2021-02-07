namespace MassTransit.Futures.Endpoints
{
    using System.Threading.Tasks;
    using Automatonymous.Activities;
    using Courier;
    using Courier.Contracts;


    public class PlanRoutingSlipExecutor<TInput> :
        IRoutingSlipExecutor<TInput>
        where TInput : class
    {
        public async Task Execute(FutureConsumeContext<TInput> context)
        {
            var factory = context.GetStateMachineActivityFactory();

            IItineraryPlanner<TInput> itineraryPlanner = factory.GetService<IItineraryPlanner<TInput>>(context);

            var trackingNumber = NewId.NextGuid();

            var builder = new RoutingSlipBuilder(trackingNumber);

            builder.AddVariable(nameof(FutureConsumeContext.FutureId), context.FutureId);

            builder.AddSubscription(context.ReceiveContext.InputAddress, RoutingSlipEvents.Completed | RoutingSlipEvents.Faulted);

            await itineraryPlanner.PlanItinerary(context, builder).ConfigureAwait(false);

            var routingSlip = builder.Build();

            await context.Execute(routingSlip).ConfigureAwait(false);

            if (TrackRoutingSlip)
                context.Instance.Pending.Add(trackingNumber);
        }

        public bool TrackRoutingSlip { get; set; }
    }
}
