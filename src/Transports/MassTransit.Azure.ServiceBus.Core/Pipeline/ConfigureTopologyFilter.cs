namespace MassTransit.Azure.ServiceBus.Core.Pipeline
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Topology;
    using Topology.Entities;
    using Util;


    public class ConfigureTopologyFilter<TSettings> :
        IFilter<ClientContext>,
        IFilter<SendEndpointContext>
        where TSettings : class
    {
        readonly BrokerTopology _brokerTopology;
        readonly bool _removeSubscriptions;
        readonly TSettings _settings;
        CancellationToken _cancellationToken;

        public ConfigureTopologyFilter(TSettings settings, BrokerTopology brokerTopology, bool removeSubscriptions, CancellationToken cancellationToken)
        {
            _settings = settings;
            _brokerTopology = brokerTopology;
            _removeSubscriptions = removeSubscriptions;
            _cancellationToken = cancellationToken;
        }

        public async Task Send(ClientContext context, IPipe<ClientContext> next)
        {
            await ConfigureTopology(context).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("configureTopology");

            _brokerTopology.Probe(scope);
        }

        public async Task Send(SendEndpointContext context, IPipe<SendEndpointContext> next)
        {
            await ConfigureTopology(context).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }

        async Task ConfigureTopology(NamespaceContext context)
        {
            await context.OneTimeSetup<ConfigureTopologyContext<TSettings>>(async payload =>
            {
                await ConfigureTopology(context.ConnectionContext).ConfigureAwait(false);

                context.GetOrAddPayload(() => _settings);

                if (_removeSubscriptions)
                {
                    var logContext = LogContext.Current;
                    if (logContext == null)
                        throw new InvalidOperationException("The LogContext must be available");

                    // ReSharper disable once MethodSupportsCancellation
                    _cancellationToken.Register(() => TaskUtil.Await(async () =>
                    {
                        LogContext.SetCurrentIfNull(logContext);

                        try
                        {
                            await RemoveSubscriptions(context.ConnectionContext).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            LogContext.Warning?.Log(ex, "Failed to remove one or more subscriptions from the endpoint.");
                        }
                    }));
                }
            }, () => new Context()).ConfigureAwait(false);
        }

        async Task ConfigureTopology(ConnectionContext context)
        {
            await Task.WhenAll(_brokerTopology.Topics.Select(topic => Create(context, topic))).ConfigureAwait(false);

            await Task.WhenAll(_brokerTopology.Queues.Select(queue => Create(context, queue))).ConfigureAwait(false);

            await Task.WhenAll(_brokerTopology.Subscriptions.Select(subscription => Create(context, subscription))).ConfigureAwait(false);

            await Task.WhenAll(_brokerTopology.QueueSubscriptions.Select(subscription => Create(context, subscription))).ConfigureAwait(false);

            await Task.WhenAll(_brokerTopology.TopicSubscriptions.Select(subscription => Create(context, subscription))).ConfigureAwait(false);
        }

        async Task RemoveSubscriptions(ConnectionContext context)
        {
            await Task.WhenAll(_brokerTopology.QueueSubscriptions.Select(subscription => Delete(context, subscription))).ConfigureAwait(false);
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
            return context.DeleteTopicSubscription(subscription.Subscription.CreateSubscriptionOptions.TopicName,
                subscription.Subscription.CreateSubscriptionOptions.SubscriptionName);
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
