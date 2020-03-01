namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using MassTransit.Configuration;


    public class BrokeredMessageReceiverServiceBusEndpointConfiguration :
        ReceiverConfiguration
    {
        public BrokeredMessageReceiverServiceBusEndpointConfiguration(IServiceBusReceiveEndpointConfiguration busConfiguration)
            : base(busConfiguration)
        {
        }
    }
}
