namespace MassTransit.MongoDbIntegration.Courier.Consumers
{
    using System.Threading.Tasks;
    using Events;
    using MassTransit.Courier.Contracts;


    public class RoutingSlipFaultedConsumer :
        IConsumer<RoutingSlipFaulted>
    {
        readonly IRoutingSlipEventPersister _persister;

        public RoutingSlipFaultedConsumer(IRoutingSlipEventPersister persister)
        {
            _persister = persister;
        }

        public Task Consume(ConsumeContext<RoutingSlipFaulted> context)
        {
            var @event = new RoutingSlipFaultedDocument(context.Message);

            return _persister.Persist(context.Message.TrackingNumber, @event);
        }
    }
}
