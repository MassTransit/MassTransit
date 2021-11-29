namespace MassTransit.AzureServiceBusTransport.Topology
{
    using Azure.Messaging.ServiceBus.Administration;


    public interface IBrokerTopologyBuilder
    {
        /// <summary>
        /// Creates a topic
        /// </summary>
        /// <param name="createTopicOptions">The immutable topic options</param>
        /// <returns>An entity handle used to reference the exchange in subsequent calls</returns>
        TopicHandle CreateTopic(CreateTopicOptions createTopicOptions);

        /// <summary>
        /// Creates a subscription
        /// </summary>
        /// <param name="topic">The source exchange</param>
        /// <param name="createSubscriptionOptions"></param>
        /// <param name="rule"></param>
        /// <param name="filter"></param>
        /// <returns>An entity handle used to reference the binding in subsequent calls</returns>
        SubscriptionHandle CreateSubscription(TopicHandle topic, CreateSubscriptionOptions createSubscriptionOptions, CreateRuleOptions rule,
            RuleFilter filter);

        /// <summary>
        /// Creates a subscription which forwards to a different topic
        /// </summary>
        /// <param name="source">The source topic</param>
        /// <param name="destination">The destination topic</param>
        /// <param name="createSubscriptionOptions"></param>
        /// <returns>An entity handle used to reference the binding in subsequent calls</returns>
        TopicSubscriptionHandle CreateTopicSubscription(TopicHandle source, TopicHandle destination, CreateSubscriptionOptions createSubscriptionOptions);

        /// <summary>
        /// Creates a queue
        /// </summary>
        /// <param name="createQueueOptions"></param>
        /// <returns></returns>
        QueueHandle CreateQueue(CreateQueueOptions createQueueOptions);

        /// <summary>
        /// Creates a subscription which forwards to a queue
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="queue"></param>
        /// <param name="createSubscriptionOptions"></param>
        /// <param name="rule"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        QueueSubscriptionHandle CreateQueueSubscription(TopicHandle exchange, QueueHandle queue, CreateSubscriptionOptions createSubscriptionOptions,
            CreateRuleOptions rule, RuleFilter filter);
    }
}
