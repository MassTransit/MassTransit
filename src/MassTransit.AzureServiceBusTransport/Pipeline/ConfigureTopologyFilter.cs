// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied. See the License for the
// specific language governing permissions and limitations under the License.
namespace MassTransit.AzureServiceBusTransport.Pipeline
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Topology;
    using Topology.Entities;


    public class ConfigureTopologyFilter<TSettings> :
        IFilter<NamespaceContext>
        where TSettings : class
    {
        readonly BrokerTopology _brokerTopology;
        readonly bool _removeSubscriptions;
        CancellationToken _cancellationToken;

        readonly TSettings _settings;

        public ConfigureTopologyFilter(TSettings settings, BrokerTopology brokerTopology, bool removeSubscriptions, CancellationToken cancellationToken)
        {
            _settings = settings;
            _brokerTopology = brokerTopology;
            _removeSubscriptions = removeSubscriptions;
            _cancellationToken = cancellationToken;
        }

        public async Task Send(NamespaceContext context, IPipe<NamespaceContext> next)
        {
            await context.OneTimeSetup<ConfigureTopologyContext<TSettings>>(async payload =>
            {
                await ConfigureTopology(context).ConfigureAwait(false);

                context.GetOrAddPayload(() => _settings);

                if (_removeSubscriptions)
                    _cancellationToken.Register(async () =>
                    {
                        try
                        {
                            await RemoveSubscriptions(context).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            LogContext.Warning?.Log(ex, "Failed to remove one or more subscriptions from the endpoint.");
                        }
                    });
            }).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("configureTopology");

            _brokerTopology.Probe(scope);
        }

        async Task ConfigureTopology(NamespaceContext context)
        {
            await Task.WhenAll(_brokerTopology.Topics.Select(topic => Create(context, topic))).ConfigureAwait(false);

            await Task.WhenAll(_brokerTopology.Queues.Select(queue => Create(context, queue))).ConfigureAwait(false);

            await Task.WhenAll(_brokerTopology.Subscriptions.Select(subscription => Create(context, subscription))).ConfigureAwait(false);

            await Task.WhenAll(_brokerTopology.QueueSubscriptions.Select(subscription => Create(context, subscription))).ConfigureAwait(false);

            await Task.WhenAll(_brokerTopology.TopicSubscriptions.Select(subscription => Create(context, subscription))).ConfigureAwait(false);
        }

        async Task RemoveSubscriptions(NamespaceContext context)
        {
            await Task.WhenAll(_brokerTopology.QueueSubscriptions.Select(subscription => Delete(context, subscription))).ConfigureAwait(false);
        }

        Task Create(NamespaceContext context, Topic topic)
        {
            return context.CreateTopic(topic.TopicDescription);
        }

        Task Create(NamespaceContext context, Queue queue)
        {
            return context.CreateQueue(queue.QueueDescription);
        }

        Task Create(NamespaceContext context, Subscription subscription)
        {
            return context.CreateTopicSubscription(subscription.SubscriptionDescription, subscription.Rule, subscription.Filter);
        }

        Task Create(NamespaceContext context, QueueSubscription subscription)
        {
            return context.CreateTopicSubscription(subscription.Subscription.SubscriptionDescription, subscription.Subscription.Rule,
                subscription.Subscription.Filter);
        }

        Task Delete(NamespaceContext context, QueueSubscription subscription)
        {
            return context.DeleteTopicSubscription(subscription.Subscription.SubscriptionDescription);
        }

        Task Create(NamespaceContext context, TopicSubscription subscription)
        {
            return context.CreateTopicSubscription(subscription.Subscription.SubscriptionDescription, subscription.Subscription.Rule,
                subscription.Subscription.Filter);
        }
    }
}
