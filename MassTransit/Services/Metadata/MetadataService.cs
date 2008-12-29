namespace MassTransit.Services.Metadata
{
	using System;
	using Server;

    public class MetadataService :
        IHostedService
    {

        private readonly IServiceBus _bus;
    	private UnsubscribeAction _unsubscribeToken;

    	public MetadataService(IServiceBus bus)
        {
            _bus = bus;
        }

        public void Start()
        {
        	_unsubscribeToken = _bus.Subscribe<MessageDefinitionConsumer>();
        	_unsubscribeToken += _bus.Subscribe<MetadataSearchConsumer>();
        }

        public void Stop()
        {
        	_unsubscribeToken();
        }

        public void Dispose()
        {

        }
    }
}