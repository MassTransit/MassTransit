namespace MassTransit.AzureServiceBusTransport.Middleware
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Logging;
    using Topology;


    public class ConfigureServiceBusTopologyFilter<TSettings> :
        IFilter<ClientContext>,
        IFilter<SendEndpointContext>
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

        public async Task Send(ClientContext context, IPipe<ClientContext> next)
        {
            OneTimeContext<ConfigureTopologyContext<TSettings>> oneTimeContext = await ConfigureTopology(context).ConfigureAwait(false);

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

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("configureTopology");

            _brokerTopology.Probe(scope);
        }

        public async Task Send(SendEndpointContext context, IPipe<SendEndpointContext> next)
        {
            OneTimeContext<ConfigureTopologyContext<TSettings>> oneTimeContext = await ConfigureTopology(context).ConfigureAwait(false);

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

        async Task<OneTimeContext<ConfigureTopologyContext<TSettings>>> ConfigureTopology(NamespaceContext context)
        {
            OneTimeContext<ConfigureTopologyContext<TSettings>> oneTimeContext = await context.OneTimeSetup<ConfigureTopologyContext<TSettings>>(() =>
            {
                context.GetOrAddPayload(() => _settings);

                if (_context != null && _removeSubscriptions)
                    _context.AddSendAgent(new RemoveServiceBusTopologyAgent(context.ConnectionContext, _brokerTopology));

                return ConfigureTopology(context.ConnectionContext);
            }).ConfigureAwait(false);

            return oneTimeContext;
        }

        async Task ConfigureTopology(ConnectionContext context)
        {
            StartedActivity? activity = LogContext.Current?.StartGenericActivity("Configure Topology");
            try
            {
                await Task.WhenAll(_brokerTopology.Topics.Select(topic => Create(context, topic))).ConfigureAwait(false);

                await Task.WhenAll(_brokerTopology.Queues.Select(queue => Create(context, queue))).ConfigureAwait(false);

                await Task.WhenAll(_brokerTopology.Subscriptions.Select(subscription => Create(context, subscription))).ConfigureAwait(false);

                await Task.WhenAll(_brokerTopology.QueueSubscriptions.Select(subscription => Create(context, subscription))).ConfigureAwait(false);

                await Task.WhenAll(_brokerTopology.TopicSubscriptions.Select(subscription => Create(context, subscription))).ConfigureAwait(false);
            }
            finally
            {
                activity?.Stop();
            }
        }

        Task Create(ConnectionContext context, Topic topic)
        {
            return context.CreateTopic(topic.CreateTopicOptions);
        }

        Task Create(ConnectionContext context, Queue queue)
        {
            return context.CreateQueue(queue.CreateQueueOptions);
        }

        Task Create(ConnectionContext context, Subscription subscription)
        {
            return context.CreateTopicSubscription(subscription.CreateSubscriptionOptions, subscription.Rule, subscription.Filter);
        }

        Task Create(ConnectionContext context, QueueSubscription subscription)
        {
            return context.CreateTopicSubscription(subscription.Subscription.CreateSubscriptionOptions, subscription.Subscription.Rule,
                subscription.Subscription.Filter);
        }

        Task Delete(ConnectionContext context, QueueSubscription subscription)
        {
            return context.DeleteTopicSubscription(subscription.Subscription.CreateSubscriptionOptions);
        }

        Task Create(ConnectionContext context, TopicSubscription subscription)
        {
            return context.CreateTopicSubscription(subscription.Subscription.CreateSubscriptionOptions, subscription.Subscription.Rule,
                subscription.Subscription.Filter);
        }


        class Context :
            ConfigureTopologyContext<TSettings>
        {
        }
    }
}
