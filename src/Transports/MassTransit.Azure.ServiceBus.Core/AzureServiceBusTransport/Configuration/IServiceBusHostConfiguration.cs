namespace MassTransit.AzureServiceBusTransport.Configuration
{
    using System;
    using MassTransit.Configuration;
    using Topology;


    public interface IServiceBusHostConfiguration :
        IHostConfiguration,
        IReceiveConfigurator<IServiceBusReceiveEndpointConfigurator>
    {
        ServiceBusHostSettings Settings { get; set; }

        string BasePath { get; }

        IConnectionContextSupervisor ConnectionContextSupervisor { get; }

        new IServiceBusBusTopology Topology { get; }

        /// <summary>
        /// Apply the endpoint definition to the receive endpoint configurator
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="definition"></param>
        void ApplyEndpointDefinition(IServiceBusReceiveEndpointConfigurator configurator, IEndpointDefinition definition);

        IServiceBusReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(string queueName,
            Action<IServiceBusReceiveEndpointConfigurator> configure = null);

        IServiceBusReceiveEndpointConfiguration CreateReceiveEndpointConfiguration(ReceiveEndpointSettings settings, IServiceBusEndpointConfiguration
            endpointConfiguration, Action<IServiceBusReceiveEndpointConfigurator> configure = null);

        IServiceBusSubscriptionEndpointConfiguration CreateSubscriptionEndpointConfiguration(SubscriptionEndpointSettings settings,
            IServiceBusEndpointConfiguration endpointConfiguration, Action<IServiceBusSubscriptionEndpointConfigurator> configure = null);

        void SubscriptionEndpoint<T>(string subscriptionName, Action<IServiceBusSubscriptionEndpointConfigurator> configure)
            where T : class;

        void SubscriptionEndpoint(string subscriptionName, string topicPath, Action<IServiceBusSubscriptionEndpointConfigurator> configure);

        void SetNamespaceSeparatorToTilde();

        void SetNamespaceSeparatorToUnderscore();

        void SetNamespaceSeparatorTo(string separator);

        IServiceBusSubscriptionEndpointConfiguration CreateSubscriptionEndpointConfiguration<T>(string subscriptionName,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure)
            where T : class;

        IServiceBusSubscriptionEndpointConfiguration CreateSubscriptionEndpointConfiguration(string subscriptionName, string topicPath,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure);
    }
}
