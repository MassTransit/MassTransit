namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Azure.Messaging.ServiceBus;
    using global::Azure.Messaging.ServiceBus.Administration;
    using GreenPipes;
    using MassTransit.Azure.ServiceBus.Core.Transport;

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

        public (ServiceBusProcessor, ServiceBusSessionProcessor) CreateQueueClient(ReceiveSettings settings)
        {
            return _context.CreateQueueClient(settings);
        }

        public (ServiceBusProcessor, ServiceBusSessionProcessor) CreateSubscriptionClient(SubscriptionSettings settings)
        {
            return _context.CreateSubscriptionClient(settings);
        }

        public ServiceBusSender CreateMessageSender(string entityPath)
        {
            return _context.CreateMessageSender(entityPath);
        }

        public Task<QueueProperties> CreateQueue(CreateQueueOptions queueDescription)
        {
            return _context.CreateQueue(queueDescription);
        }

        public Task<TopicProperties> CreateTopic(CreateTopicOptions topicDescription)
        {
            return _context.CreateTopic(topicDescription);
        }

        public Task<SubscriptionProperties> CreateTopicSubscription(CreateSubscriptionOptions subscriptionDescription, CreateRuleOptions rule, RuleFilter filter)
        {
            return _context.CreateTopicSubscription(subscriptionDescription, rule, filter);
        }

        public Task DeleteTopicSubscription(CreateSubscriptionOptions description)
        {
            return _context.DeleteTopicSubscription(description);
        }

        Uri ConnectionContext.Endpoint => _context.Endpoint;
    }
}
