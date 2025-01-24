#nullable enable
namespace MassTransit.SqlTransport.Topology
{
    using System;


    public interface IBrokerTopologyBuilder
    {
        /// <summary>
        /// Declares an exchange
        /// </summary>
        /// <param name="name">The exchange name</param>
        /// <returns>An entity handle used to reference the exchange in subsequent calls</returns>
        TopicHandle CreateTopic(string name);

        /// <summary>
        /// Bind an exchange to an exchange, with the specified routing key and arguments
        /// </summary>
        /// <param name="source">The source exchange</param>
        /// <param name="destination">The destination exchange</param>
        /// <param name="subscriptionType"></param>
        /// <param name="routingKey">The binding routing key</param>
        /// <returns>An entity handle used to reference the binding in subsequent calls</returns>
        TopicSubscriptionHandle CreateTopicSubscription(TopicHandle source, TopicHandle destination,
            SqlSubscriptionType subscriptionType = SqlSubscriptionType.All, string? routingKey = null);

        /// <summary>
        /// Declares a queue
        /// </summary>
        /// <param name="name"></param>
        /// <param name="autoDeleteOnIdle"></param>
        /// <param name="maxDeliveryCount"></param>
        /// <returns></returns>
        QueueHandle CreateQueue(string name, TimeSpan? autoDeleteOnIdle = null, int? maxDeliveryCount = null);

        /// <summary>
        /// Binds an exchange to a queue, with the specified routing key and arguments
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="queue"></param>
        /// <param name="subscriptionType"></param>
        /// <param name="routingKey"></param>
        /// <returns></returns>
        QueueSubscriptionHandle CreateQueueSubscription(TopicHandle topic, QueueHandle queue,
            SqlSubscriptionType subscriptionType = SqlSubscriptionType.All, string? routingKey = null);
    }
}
