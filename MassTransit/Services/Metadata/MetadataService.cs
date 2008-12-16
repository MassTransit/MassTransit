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
            _bus.Subscribe<MessageConsumer>();
        }

        public void Stop()
        {
            _bus.Unsubscribe<MessageConsumer>();
        }

        public void Dispose()
        {

        }
    }
}