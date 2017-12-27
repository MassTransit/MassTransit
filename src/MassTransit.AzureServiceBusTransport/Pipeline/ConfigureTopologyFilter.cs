// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading.Tasks;
    using GreenPipes;
    using Logging;
    using MassTransit.Pipeline;
    using Topology;
    using Topology.Entities;


    public class ConfigureTopologyFilter<TSettings> :
        IFilter<NamespaceContext>
        where TSettings : class
    {
        readonly ILog _log = Logger.Get<ConfigureTopologyFilter<TSettings>>();
        readonly bool _removeSubscriptions;

        readonly TSettings _settings;
        readonly BrokerTopology _brokerTopology;

        public ConfigureTopologyFilter(TSettings settings, BrokerTopology brokerTopology, bool removeSubscriptions)
        {
            _settings = settings;
            _brokerTopology = brokerTopology;
            _removeSubscriptions = removeSubscriptions;
        }

        public async Task Send(NamespaceContext context, IPipe<NamespaceContext> next)
        {
            await context.OneTimeSetup<ConfigureTopologyContext>(async payload =>
            {
                await ConfigureTopology(context).ConfigureAwait(false);

                context.GetOrAddPayload(() => _settings);
            }).ConfigureAwait(false);

            try
            {
                await next.Send(context).ConfigureAwait(false);
            }
            finally
            {
                if (_removeSubscriptions)
                {
                    try
                    {
                        await RemoveSubscriptions(context).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        if (_log.IsWarnEnabled)
                            _log.Warn("Failed to remove one or more subsriptions from the endpoint.", ex);
                    }
                }
            }
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
            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Create Topic ({0})", topic);
            }

            return context.CreateTopic(topic.TopicDescription);
        }

        Task Create(NamespaceContext context, Queue queue)
        {
            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Create Queue ({0})", queue);
            }

            return context.CreateQueue(queue.QueueDescription);
        }

        Task Create(NamespaceContext context, Subscription subscription)
        {
            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Create Subscription ({0})", subscription);
            }

            return context.CreateTopicSubscription(subscription.SubscriptionDescription);
        }

        Task Create(NamespaceContext context, QueueSubscription subscription)
        {
            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Create Queue Subscription ({0})", subscription);
            }

            // TODO we need to deal with scopes better to make this more realistic
            var queuePath = $"{context.NamespaceManager.Address.AbsolutePath.TrimStart('/')}{subscription.Destination.QueueDescription.Path}";

            subscription.Subscription.SubscriptionDescription.ForwardTo = queuePath;

            return context.CreateTopicSubscription(subscription.Subscription.SubscriptionDescription);
        }

        Task Delete(NamespaceContext context, QueueSubscription subscription)
        {
            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Create Queue Subscription ({0})", subscription);
            }

            return context.DeleteTopicSubscription(subscription.Subscription.SubscriptionDescription);
        }

        Task Create(NamespaceContext context, TopicSubscription subscription)
        {
            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Create Topic Subscription ({0})", subscription);
            }

            return context.CreateTopicSubscription(subscription.Subscription.SubscriptionDescription);
        }


        public interface ConfigureTopologyContext
        {
        }
    }
}