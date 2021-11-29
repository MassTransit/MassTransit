namespace MassTransit.MongoDbIntegration.Courier.Consumers
{
    using System.Threading.Tasks;
    using Events;
    using MassTransit.Courier.Contracts;


    public class RoutingSlipActivityCompensatedConsumer :
        IConsumer<RoutingSlipActivityCompensated>
    {
        readonly IRoutingSlipEventPersister _persister;

        public RoutingSlipActivityCompensatedConsumer(IRoutingSlipEventPersister persister)
        {
            _persister = persister;
        }

        public Task Consume(ConsumeContext<RoutingSlipActivityCompensated> context)
        {
            var @event = new RoutingSlipActivityCompensatedDocument(context.Message);

            return _persister.Persist(context.Message.TrackingNumber, @event);
        }
    }
}
