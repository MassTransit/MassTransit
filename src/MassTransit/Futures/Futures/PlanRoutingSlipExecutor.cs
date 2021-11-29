namespace MassTransit.Futures
{
    using System.Threading.Tasks;
    using Courier;
    using Courier.Contracts;
    using SagaStateMachine;


    public class PlanRoutingSlipExecutor<TInput> :
        IRoutingSlipExecutor<TInput>
        where TInput : class
    {
        public async Task Execute(BehaviorContext<FutureState, TInput> context)
        {
            var factory = context.GetStateMachineActivityFactory();

            var itineraryPlanner = factory.GetService<IItineraryPlanner<TInput>>(context);

            var trackingNumber = NewId.NextGuid();

            var builder = new RoutingSlipBuilder(trackingNumber);

            builder.AddVariable(MessageHeaders.FutureId, context.CorrelationId);

            builder.AddSubscription(context.ReceiveContext.InputAddress, RoutingSlipEvents.Completed | RoutingSlipEvents.Faulted);

            await itineraryPlanner.PlanItinerary(context, builder).ConfigureAwait(false);

            var routingSlip = builder.Build();

            await context.Execute(routingSlip).ConfigureAwait(false);

            if (TrackRoutingSlip)
                context.Saga.Pending.Add(trackingNumber);
        }

        public bool TrackRoutingSlip { get; set; }
    }
}
