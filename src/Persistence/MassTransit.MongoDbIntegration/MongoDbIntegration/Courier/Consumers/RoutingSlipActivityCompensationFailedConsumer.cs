namespace MassTransit.MongoDbIntegration.Courier.Consumers
{
    using System.Threading.Tasks;
    using Events;
    using MassTransit.Courier.Contracts;


    public class RoutingSlipActivityCompensationFailedConsumer :
        IConsumer<RoutingSlipActivityCompensationFailed>
    {
        readonly IRoutingSlipEventPersister _persister;

        public RoutingSlipActivityCompensationFailedConsumer(IRoutingSlipEventPersister persister)
        {
            _persister = persister;
        }

        public Task Consume(ConsumeContext<RoutingSlipActivityCompensationFailed> context)
        {
            var @event = new RoutingSlipActivityCompensationFailedDocument(context.Message);

            return _persister.Persist(context.Message.TrackingNumber, @event);
        }
    }
}
