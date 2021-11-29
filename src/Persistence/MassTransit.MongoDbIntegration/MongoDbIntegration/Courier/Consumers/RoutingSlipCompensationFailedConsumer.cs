namespace MassTransit.MongoDbIntegration.Courier.Consumers
{
    using System.Threading.Tasks;
    using Events;
    using MassTransit.Courier.Contracts;


    public class RoutingSlipCompensationFailedConsumer :
        IConsumer<RoutingSlipCompensationFailed>
    {
        readonly IRoutingSlipEventPersister _persister;

        public RoutingSlipCompensationFailedConsumer(IRoutingSlipEventPersister persister)
        {
            _persister = persister;
        }

        public Task Consume(ConsumeContext<RoutingSlipCompensationFailed> context)
        {
            var @event = new RoutingSlipCompensationFailedDocument(context.Message);

            return _persister.Persist(context.Message.TrackingNumber, @event);
        }
    }
}
