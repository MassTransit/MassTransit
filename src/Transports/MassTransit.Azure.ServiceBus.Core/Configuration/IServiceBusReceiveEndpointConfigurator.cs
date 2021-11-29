namespace MassTransit
{
    using System;


    /// <summary>
    /// Configure an Azure Service Bus receive endpoint
    /// </summary>
    public interface IServiceBusReceiveEndpointConfigurator :
        IReceiveEndpointConfigurator,
        IServiceBusQueueEndpointConfigurator
    {
        /// <summary>
        /// If true, on shutdown, the subscriptions added are removed. This is used to avoid auto-delete
        /// queues from creating abandoned subscriptions on the topic, resulting in a quota overflow.
        /// </summary>
        bool RemoveSubscriptions { set; }

        /// <summary>
        /// Create a topic subscription on the endpoint
        /// </summary>
        /// <param name="topicName">The topic name</param>
        /// <param name="subscriptionName">The name for the subscription</param>
        /// <param name="callback">Configure the exchange and binding</param>
        void Subscribe(string topicName, string subscriptionName, Action<IServiceBusSubscriptionConfigurator> callback = null);

        /// <summary>
        /// Create a topic subscription for the message type
        /// </summary>
        /// <param name="subscriptionName">The name for the subscription</param>
        /// <param name="callback">Configure the topic subscription</param>
        void Subscribe<T>(string subscriptionName, Action<IServiceBusSubscriptionConfigurator> callback = null)
            where T : class;
    }
}
