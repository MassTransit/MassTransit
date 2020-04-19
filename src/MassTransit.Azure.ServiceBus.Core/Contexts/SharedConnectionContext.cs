namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Core;
    using Microsoft.Azure.ServiceBus.Management;


    public class SharedConnectionContext :
        ProxyPipeContext,
        ConnectionContext
    {
        readonly ConnectionContext _context;

        public SharedConnectionContext(ConnectionContext context, CancellationToken cancellationToken)
            : base(context)
        {
            _context = context;
            CancellationToken = cancellationToken;
        }

        public override CancellationToken CancellationToken { get; }

        public IQueueClient CreateQueueClient(string entityPath)
        {
            return _context.CreateQueueClient(entityPath);
        }

        public ISubscriptionClient CreateSubscriptionClient(string topicPath, string subscriptionName)
        {
            return _context.CreateSubscriptionClient(topicPath, subscriptionName);
        }

        public IMessageSender CreateMessageSender(string entityPath)
        {
            return _context.CreateMessageSender(entityPath);
        }

        public Task<QueueDescription> CreateQueue(QueueDescription queueDescription)
        {
            return _context.CreateQueue(queueDescription);
        }

        public Task<TopicDescription> CreateTopic(TopicDescription topicDescription)
        {
            return _context.CreateTopic(topicDescription);
        }

        public Task<SubscriptionDescription> CreateTopicSubscription(SubscriptionDescription subscriptionDescription, RuleDescription rule, Filter filter)
        {
            return _context.CreateTopicSubscription(subscriptionDescription, rule, filter);
        }

        public Task DeleteTopicSubscription(SubscriptionDescription description)
        {
            return _context.DeleteTopicSubscription(description);
        }

        Uri ConnectionContext.Endpoint => _context.Endpoint;
    }
}
