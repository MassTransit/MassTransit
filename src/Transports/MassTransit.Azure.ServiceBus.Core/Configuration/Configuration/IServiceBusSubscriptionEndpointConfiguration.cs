namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using Transport;


    public interface IServiceBusSubscriptionEndpointConfiguration :
        IServiceBusEntityEndpointConfiguration
    {
        SubscriptionSettings Settings { get; }
    }
}
