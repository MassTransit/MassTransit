namespace MassTransit.Azure.ServiceBus.Core.Configuration
{
    using System;
    using MassTransit.Configuration;
    using Settings;


    public interface IServiceBusHostConfiguration :
        IHostConfiguration,
        IReceiveConfigurator<IServiceBusReceiveEndpointConfigurator>
    {
        ServiceBusHostSettings Settings { get; set; }

        string BasePath { get; }

        bool DeployTopologyOnly { get; set; }

        IServiceBusHost Proxy { get; }

        IServiceBusReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            Action<IServiceBusReceiveEndpointConfigurator> configure = null);

        IServiceBusReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(ReceiveEndpointSettings settings, IServiceBusEndpointConfiguration
            endpointConfiguration, Action<IServiceBusReceiveEndpointConfigurator> configure = null);

        IServiceBusSubscriptionEndpointConfiguration CreateSubscriptionEndpointConfiguration(SubscriptionEndpointSettings settings,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure = null);

        void SubscriptionEndpoint<T>(string subscriptionName, Action<IServiceBusSubscriptionEndpointConfigurator> configure)
            where T : class;

        void SubscriptionEndpoint(string subscriptionName, string topicPath, Action<IServiceBusSubscriptionEndpointConfigurator> configure);
    }
}
