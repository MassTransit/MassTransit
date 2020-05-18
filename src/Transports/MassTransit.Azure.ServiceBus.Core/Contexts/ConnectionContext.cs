namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Core;
    using Microsoft.Azure.ServiceBus.Management;


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

        IQueueClient CreateQueueClient(string entityPath);

        ISubscriptionClient CreateSubscriptionClient(string topicPath, string subscriptionName);

        IMessageSender CreateMessageSender(string entityPath);

        /// <summary>
        /// Create a queue in the host namespace (which is scoped to the full ServiceUri)
        /// </summary>
        /// <param name="queueDescription"></param>
        /// <returns></returns>
        Task<QueueDescription> CreateQueue(QueueDescription queueDescription);

        /// <summary>
        /// Create a topic in the root namespace
        /// </summary>
        /// <param name="topicDescription"></param>
        /// <returns></returns>
        Task<TopicDescription> CreateTopic(TopicDescription topicDescription);

        /// <summary>
        /// Create a topic subscription
        /// </summary>
        /// <param name="subscriptionDescription"></param>
        /// <param name="rule"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<SubscriptionDescription> CreateTopicSubscription(SubscriptionDescription subscriptionDescription, RuleDescription rule, Filter filter);

        /// <summary>
        /// Delete a subscription from the topic
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        Task DeleteTopicSubscription(SubscriptionDescription description);
    }
}
