namespace MassTransit.Metadata
{
    using Server;
    
    public class MetadataService :
        IHostedService
    {

        private readonly IServiceBus _bus;

        public MetadataService(IServiceBus bus)
        {
            _bus = bus;
        }

        public void Start()
        {
            _bus.Subscribe<MessageConsumer>();
            _bus.Subscribe<EndpointConsumer>();
            _bus.Subscribe<TransmissionConsumer>();
        }

        public void Stop()
        {
            _bus.Unsubscribe<EndpointConsumer>();
            _bus.Unsubscribe<MessageConsumer>();
            _bus.Unsubscribe<TransmissionConsumer>();
        }

        public void Dispose()
        {

        }
    }
}