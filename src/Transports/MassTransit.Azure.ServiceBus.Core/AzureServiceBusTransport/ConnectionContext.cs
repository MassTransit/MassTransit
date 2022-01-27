namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using Azure.Messaging.ServiceBus.Administration;


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
        /// <param name="createQueueOptions"></param>
        /// <returns></returns>
        Task<QueueProperties> CreateQueue(CreateQueueOptions createQueueOptions);

        /// <summary>
        /// Create a topic in the root namespace
        /// </summary>
        /// <param name="createTopicOptions"></param>
        /// <returns></returns>
        Task<TopicProperties> CreateTopic(CreateTopicOptions createTopicOptions);

        /// <summary>
        /// Create a topic subscription
        /// </summary>
        /// <param name="createSubscriptionOptions"></param>
        /// <param name="rule"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<SubscriptionProperties> CreateTopicSubscription(CreateSubscriptionOptions createSubscriptionOptions, CreateRuleOptions rule, RuleFilter filter);

        /// <summary>
        /// Delete a subscription from the topic
        /// </summary>
        /// <param name="subscriptionOptions"></param>
        /// <returns></returns>
        Task DeleteTopicSubscription(CreateSubscriptionOptions subscriptionOptions);
    }
}
