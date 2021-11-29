namespace MassTransit.MongoDbIntegration.Courier.Consumers
{
    using System.Threading.Tasks;
    using Events;
    using MassTransit.Courier.Contracts;


    public class RoutingSlipTerminatedConsumer :
        IConsumer<RoutingSlipTerminated>
    {
        readonly IRoutingSlipEventPersister _persister;

        public RoutingSlipTerminatedConsumer(IRoutingSlipEventPersister persister)
        {
            _persister = persister;
        }

        public Task Consume(ConsumeContext<RoutingSlipTerminated> context)
        {
            var @event = new RoutingSlipTerminatedDocument(context.Message);

            return _persister.Persist(context.Message.TrackingNumber, @event);
        }
    }
}
