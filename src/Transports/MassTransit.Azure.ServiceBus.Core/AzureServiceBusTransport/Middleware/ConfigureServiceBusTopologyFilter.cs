namespace MassTransit.AzureServiceBusTransport.Middleware
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using Topology;


    public class ConfigureServiceBusTopologyFilter<TSettings> :
        IFilter<ClientContext>
        where TSettings : class
    {
        readonly BrokerTopology _brokerTopology;
        readonly ServiceBusReceiveEndpointContext _context;
        readonly bool _removeSubscriptions;
        readonly TSettings _settings;

        public ConfigureServiceBusTopologyFilter(TSettings settings, BrokerTopology brokerTopology, bool removeSubscriptions = false,
            ServiceBusReceiveEndpointContext context = null)
        {
            _settings = settings;
            _brokerTopology = brokerTopology;
            _removeSubscriptions = removeSubscriptions;
            _context = context;
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("configureTopology");

            _brokerTopology.Probe(scope);
        }

        public async Task Send(ClientContext context, IPipe<ClientContext> next)
        {
            OneTimeContext<ConfigureTopologyContext<TSettings>> oneTimeContext = await Configure(context, context.CancellationToken).ConfigureAwait(false);

            try
            {
                await next.Send(context).ConfigureAwait(false);
            }
            catch (Exception)
            {
                oneTimeContext.Evict();

                throw;
            }
        }

        public async Task<OneTimeContext<ConfigureTopologyContext<TSettings>>> Configure(NamespaceContext context, CancellationToken cancellationToken)
        {
            OneTimeContext<ConfigureTopologyContext<TSettings>> oneTimeContext = await context.OneTimeSetup<ConfigureTopologyContext<TSettings>>(() =>
            {
                context.GetOrAddPayload(() => _settings);

                if (_context != null && _removeSubscriptions)
                    _context.AddSendAgent(new RemoveServiceBusTopologyAgent(context.ConnectionContext, _brokerTopology));

                return ConfigureTopology(context.ConnectionContext, cancellationToken);
            }).ConfigureAwait(false);

            return oneTimeContext;
        }

        async Task ConfigureTopology(ConnectionContext context, CancellationToken cancellationToken)
        {
            StartedActivity? activity = LogContext.Current?.StartGenericActivity("Configure Topology");
            try
            {
                await Task.WhenAll(_brokerTopology.Topics.Select(topic => Create(context, topic, cancellationToken))).ConfigureAwait(false);

                await Task.WhenAll(_brokerTopology.Queues.Select(queue => Create(context, queue, cancellationToken))).ConfigureAwait(false);

                await Task.WhenAll(_brokerTopology.Subscriptions.Select(subscription => Create(context, subscription, cancellationToken)))
                    .ConfigureAwait(false);

                await Task.WhenAll(_brokerTopology.QueueSubscriptions.Select(subscription => Create(context, subscription, cancellationToken)))
                    .ConfigureAwait(false);

                await Task.WhenAll(_brokerTopology.TopicSubscriptions.Select(subscription => Create(context, subscription, cancellationToken)))
                    .ConfigureAwait(false);
            }
            finally
            {
                activity?.Stop();
            }
        }

        Task Create(ConnectionContext context, Topic topic, CancellationToken cancellationToken)
        {
            return context.CreateTopic(topic.CreateTopicOptions, cancellationToken);
        }

        Task Create(ConnectionContext context, Queue queue, CancellationToken cancellationToken)
        {
            return context.CreateQueue(queue.CreateQueueOptions, cancellationToken);
        }

        Task Create(ConnectionContext context, Subscription subscription, CancellationToken cancellationToken)
        {
            return context.CreateTopicSubscription(subscription.CreateSubscriptionOptions, subscription.Rule, subscription.Filter, cancellationToken);
        }

        Task Create(ConnectionContext context, QueueSubscription subscription, CancellationToken cancellationToken)
        {
            return context.CreateTopicSubscription(subscription.Subscription.CreateSubscriptionOptions, subscription.Subscription.Rule,
                subscription.Subscription.Filter, cancellationToken);
        }

        Task Delete(ConnectionContext context, QueueSubscription subscription, CancellationToken cancellationToken)
        {
            return context.DeleteTopicSubscription(subscription.Subscription.CreateSubscriptionOptions, cancellationToken);
        }

        Task Create(ConnectionContext context, TopicSubscription subscription, CancellationToken cancellationToken)
        {
            return context.CreateTopicSubscription(subscription.Subscription.CreateSubscriptionOptions, subscription.Subscription.Rule,
                subscription.Subscription.Filter, cancellationToken);
        }
    }
}
