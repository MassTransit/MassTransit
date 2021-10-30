namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Threading.Tasks;
    using global::Azure.Messaging.ServiceBus;
    using global::Azure.Messaging.ServiceBus.Administration;
    using GreenPipes;
    using MassTransit.Azure.ServiceBus.Core.Transport;


    /// <summary>
    /// Service Bus Connection Context
    /// </summary>
    public interface ConnectionContext :
        PipeContext
    {
        /// <summary>
        /// The Azure Service Bus endpoint, which is a Uri, but without any path information.
        /// </summary>
        Uri Endpoint { get; }

        ServiceBusProcessor CreateQueueProcessor(ReceiveSettings settings);
        ServiceBusSessionProcessor CreateQueueSessionProcessor(ReceiveSettings settings);

        ServiceBusProcessor CreateSubscriptionProcessor(SubscriptionSettings settings);
        ServiceBusSessionProcessor CreateSubscriptionSessionProcessor(SubscriptionSettings settings);

        ServiceBusSender CreateMessageSender(string entityPath);

        /// <summary>
        /// Create a queue in the host namespace (which is scoped to the full ServiceUri)
        /// </summary>
        /// <param name="queueDescription"></param>
        /// <returns></returns>
        Task<QueueProperties> CreateQueue(CreateQueueOptions queueDescription);

        /// <summary>
        /// Create a topic in the root namespace
        /// </summary>
        /// <param name="topicDescription"></param>
        /// <returns></returns>
        Task<TopicProperties> CreateTopic(CreateTopicOptions topicDescription);

        /// <summary>
        /// Create a topic subscription
        /// </summary>
        /// <param name="subscriptionDescription"></param>
        /// <param name="rule"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<SubscriptionProperties> CreateTopicSubscription(CreateSubscriptionOptions subscriptionDescription, CreateRuleOptions rule, RuleFilter filter);

        /// <summary>
        /// Delete a subscription from the topic
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="subscriptionName"></param>
        /// <returns></returns>
        Task DeleteTopicSubscription(string topicName, string subscriptionName);
    }
}
