namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using Transports;


    /// <summary>
    /// An Azure ServiceBus Host, which caches the messaging factory and namespace manager
    /// </summary>
    public interface IServiceBusHost :
        IHost<IServiceBusReceiveEndpointConfigurator>
    {
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
