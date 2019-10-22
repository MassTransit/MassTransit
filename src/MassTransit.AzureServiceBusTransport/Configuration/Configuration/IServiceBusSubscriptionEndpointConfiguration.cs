namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using Transport;


    public interface IServiceBusSubscriptionEndpointConfiguration :
        IServiceBusEntityEndpointConfiguration
    {
        SubscriptionSettings Settings { get; }
    }
}
