namespace MassTransit.MongoDbIntegration.Courier.Consumers
{
    using System.Threading.Tasks;
    using Events;
    using MassTransit.Courier.Contracts;


    public class RoutingSlipRevisedConsumer :
        IConsumer<RoutingSlipRevised>
    {
        readonly IRoutingSlipEventPersister _persister;

        public RoutingSlipRevisedConsumer(IRoutingSlipEventPersister persister)
        {
            _persister = persister;
        }

        public Task Consume(ConsumeContext<RoutingSlipRevised> context)
        {
            var @event = new RoutingSlipRevisedDocument(context.Message);

            return _persister.Persist(context.Message.TrackingNumber, @event);
        }
    }
}
