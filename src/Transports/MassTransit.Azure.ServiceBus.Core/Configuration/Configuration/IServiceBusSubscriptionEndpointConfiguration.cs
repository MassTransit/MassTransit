namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using Transport;


    public interface IServiceBusSubscriptionEndpointConfiguration :
        IServiceBusEntityEndpointConfiguration
    {
        IServiceBusSubscriptionEndpointConfigurator Configurator { get; }

        SubscriptionSettings Settings { get; }
    }
}
