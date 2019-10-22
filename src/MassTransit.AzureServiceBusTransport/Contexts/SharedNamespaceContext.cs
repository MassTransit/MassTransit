namespace MassTransit.AzureServiceBusTransport.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.ServiceBus.Messaging;


    public class SharedNamespaceContext :
        ScopePipeContext,
        NamespaceContext
    {
        readonly NamespaceContext _context;
        readonly CancellationToken _cancellationToken;

        public SharedNamespaceContext(NamespaceContext context, CancellationToken cancellationToken)
            : base(context)
        {
            _context = context;
            _cancellationToken = cancellationToken;
        }

        CancellationToken PipeContext.CancellationToken => _cancellationToken;

        Uri NamespaceContext.ServiceAddress => _context.ServiceAddress;

        Task<QueueDescription> NamespaceContext.CreateQueue(QueueDescription queueDescription)
        {
            return _context.CreateQueue(queueDescription);
        }

        Task<TopicDescription> NamespaceContext.CreateTopic(TopicDescription topicDescription)
        {
            return _context.CreateTopic(topicDescription);
        }

        Task<SubscriptionDescription> NamespaceContext.CreateTopicSubscription(SubscriptionDescription subscriptionDescription, RuleDescription rule, Filter filter)
        {
            return _context.CreateTopicSubscription(subscriptionDescription, rule, filter);
        }

        Task NamespaceContext.DeleteTopicSubscription(SubscriptionDescription description)
        {
            return _context.DeleteTopicSubscription(description);
        }
    }
}
