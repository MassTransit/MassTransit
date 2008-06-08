namespace HeavyLoad.Correlated
{
    using MassTransit.ServiceBus;

    internal class SimpleRequestService : 
        Consumes<SimpleRequestMessage>.All
    {
        private readonly IServiceBus _bus;

        public SimpleRequestService(IServiceBus bus)
        {
            _bus = bus;
        }

        public void Consume(SimpleRequestMessage message)
        {
            _bus.Publish(new SimpleResponseMessage(message.CorrelationId));
        }
    }
}