namespace MassTransit.Services.Metadata
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
            _bus.Subscribe<MessageDefinitionConsumer>();
            _bus.Subscribe<MetadataSearchConsumer>();
        }

        public void Stop()
        {
            _bus.Unsubscribe<MessageDefinitionConsumer>();
            _bus.Unsubscribe<MetadataSearchConsumer>();
        }

        public void Dispose()
        {

        }
    }
}