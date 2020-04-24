namespace MassTransit.Azure.ServiceBus.Core
{
    using System;
    using Topology;


    /// <summary>
    /// Configure an Azure Service Bus receive endpoint
    /// </summary>
    public interface IServiceBusReceiveEndpointConfigurator :
        IReceiveEndpointConfigurator,
        IServiceBusQueueEndpointConfigurator
    {
        /// <summary>
        /// If true, adds subscriptions for the message types to the related topics.
        /// </summary>
        [Obsolete("Use ConfigureConsumeTopology instead. In the meantime, this property sets that property as well.")]
        bool SubscribeMessageTopics { set; }

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
        void Subscribe(string topicName, string subscriptionName, Action<ISubscriptionConfigurator> callback = null);

        /// <summary>
        /// Create a topic subscription for the message type
        /// </summary>
        /// <param name="subscriptionName">The name for the subscription</param>
        /// <param name="callback">Configure the topic subscription</param>
        void Subscribe<T>(string subscriptionName, Action<ISubscriptionConfigurator> callback = null)
            where T : class;
    }
}
