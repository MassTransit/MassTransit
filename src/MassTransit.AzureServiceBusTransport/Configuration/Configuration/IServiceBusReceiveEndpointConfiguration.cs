namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using Transport;


    public interface IServiceBusReceiveEndpointConfiguration :
        IServiceBusEntityEndpointConfiguration
    {
        bool SubscribeMessageTopics { get; }

        ReceiveSettings Settings { get; }
    }
}
