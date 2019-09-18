namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using Transport;


    public interface IServiceBusReceiveEndpointConfiguration :
        IServiceBusEntityEndpointConfiguration
    {
        bool SubscribeMessageTopics { get; }

        ReceiveSettings Settings { get; }
    }
}
