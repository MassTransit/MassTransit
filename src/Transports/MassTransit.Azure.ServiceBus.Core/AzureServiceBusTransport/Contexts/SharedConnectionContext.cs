namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using Azure.Messaging.ServiceBus.Administration;
    using MassTransit.Middleware;


    public class SharedConnectionContext :
        ProxyPipeContext,
        ConnectionContext,
        IDisposable
    {
        readonly CancellationToken _cancellationToken;
        readonly ConnectionContext _context;
        CancellationTokenSource _tokenSource;

        public SharedConnectionContext(ConnectionContext context, CancellationToken cancellationToken)
            : base(context)
        {
            _context = context;

            _cancellationToken = cancellationToken;
            _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken, cancellationToken);
        }

        public override CancellationToken CancellationToken => _tokenSource?.Token ?? _cancellationToken;

        public Uri Endpoint => _context.Endpoint;

        public ServiceBusProcessor CreateQueueProcessor(ReceiveSettings settings)
        {
            return _context.CreateQueueProcessor(settings);
        }

        public ServiceBusSessionProcessor CreateQueueSessionProcessor(ReceiveSettings settings)
        {
            return _context.CreateQueueSessionProcessor(settings);
        }

        public ServiceBusProcessor CreateSubscriptionProcessor(SubscriptionSettings settings)
        {
            return _context.CreateSubscriptionProcessor(settings);
        }

        public ServiceBusSessionProcessor CreateSubscriptionSessionProcessor(SubscriptionSettings settings)
        {
            return _context.CreateSubscriptionSessionProcessor(settings);
        }

        public ServiceBusSender CreateMessageSender(string entityPath)
        {
            return _context.CreateMessageSender(entityPath);
        }

        public Task<QueueProperties> CreateQueue(CreateQueueOptions createQueueOptions, CancellationToken cancellationToken)
        {
            using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

            return _context.CreateQueue(createQueueOptions, tokenSource.Token);
        }

        public Task<TopicProperties> CreateTopic(CreateTopicOptions createTopicOptions, CancellationToken cancellationToken)
        {
            using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

            return _context.CreateTopic(createTopicOptions, tokenSource.Token);
        }

        public Task<SubscriptionProperties> CreateTopicSubscription(CreateSubscriptionOptions createSubscriptionOptions, CreateRuleOptions rule,
            RuleFilter filter, CancellationToken cancellationToken)
        {
            using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

            return _context.CreateTopicSubscription(createSubscriptionOptions, rule, filter, tokenSource.Token);
        }

        public Task DeleteTopicSubscription(CreateSubscriptionOptions subscriptionOptions, CancellationToken cancellationToken)
        {
            using var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken, cancellationToken);

            return _context.DeleteTopicSubscription(subscriptionOptions, tokenSource.Token);
        }

        public void Dispose()
        {
            _tokenSource?.Dispose();
            _tokenSource = null;
        }
    }
}
