namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using GreenPipes;
    using Pipeline;
    using Topology;


    /// <summary>
    /// An Azure ServiceBus Host, which caches the messaging factory and namespace manager
    /// </summary>
    public interface IServiceBusHost :
        IHost,
        IReceiveConnector<IServiceBusReceiveEndpointConfigurator>
    {
        /// <summary>
        /// The default messaging factory cache, could be AMQP or NET-TCP, depending upon configuration
        /// </summary>
        IMessagingFactoryContextSupervisor MessagingFactoryContextSupervisor { get; }

        /// <summary>
        /// The namespace cache for operating on the service bus namespace (management)
        /// </summary>
        INamespaceContextSupervisor NamespaceContextSupervisor { get; }

        /// <summary>
        /// The retry policy shared by transports communicating with the host. Should be
        /// used for all operations against Azure.
        /// </summary>
        IRetryPolicy RetryPolicy { get; }

        ServiceBusHostSettings Settings { get; }

        new IServiceBusHostTopology Topology { get; }

        /// <summary>
        /// Create a subscription endpoint on the host, which can be stopped independently from the bus
        /// </summary>
        /// <typeparam name="T">The topic message type</typeparam>
        /// <param name="subscriptionName">The subscription name for this endpoint</param>
        /// <param name="configure">Configuration callback for the endpoint</param>
        /// <returns></returns>
        HostReceiveEndpointHandle ConnectSubscriptionEndpoint<T>(string subscriptionName, Action<IServiceBusSubscriptionEndpointConfigurator> configure = null)
            where T : class;

        /// <summary>
        /// Create a subscription endpoint on the host, which can be stopped independently from the bus
        /// </summary>
        /// <param name="subscriptionName">The subscription name for this endpoint</param>
        /// <param name="topicName">The topic name to subscribe for this endpoint</param>
        /// <param name="configure">Configuration callback for the endpoint</param>
        /// <returns></returns>
        HostReceiveEndpointHandle ConnectSubscriptionEndpoint(string subscriptionName, string topicName,
            Action<IServiceBusSubscriptionEndpointConfigurator> configure = null);
    }
}
