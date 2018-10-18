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
namespace MassTransit.AmazonSqsTransport.Pipeline
{
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;
    using Logging;
    using Topology;
    using Topology.Builders;
    using Topology.Entities;

    /// <summary>
    /// Configures the broker with the supplied topology once the model is created, to ensure
    /// that the exchanges, queues, and bindings for the model are properly configured in AmazonSQS.
    /// </summary>
    public class ConfigureTopologyFilter<TSettings> :
        IFilter<ClientContext>
        where TSettings : class
    {
        readonly BrokerTopology _brokerTopology;
        readonly ILog _log = Logger.Get<ConfigureTopologyFilter<TSettings>>();
        readonly TSettings _settings;

        public ConfigureTopologyFilter(TSettings settings, BrokerTopology brokerTopology)
        {
            _settings = settings;
            _brokerTopology = brokerTopology;
        }

        async Task IFilter<ClientContext>.Send(ClientContext context, IPipe<ClientContext> next)
        {
            await context.OneTimeSetup<ConfigureTopologyContext<TSettings>>(async payload =>
            {
                await ConfigureTopology(context).ConfigureAwait(false);

                context.GetOrAddPayload(() => _settings);
            }).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);

            if (_settings is ReceiveSettings)
                await DeleteAutoDelete(context).ConfigureAwait(false);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("configureTopology");

            _brokerTopology.Probe(scope);
        }

        async Task ConfigureTopology(ClientContext context)
        {
            var topics = _brokerTopology.Topics.Select(topic => Declare(context, topic));

            var queues = _brokerTopology.Queues.Select(queue => Declare(context, queue));

            var subscriptions = _brokerTopology.QueueSubscriptions.Select(queue => Declare(context, queue));

            await Task.WhenAll(topics.Concat(queues).Concat(subscriptions)).ConfigureAwait(false);
        }

        async Task DeleteAutoDelete(ClientContext context)
        {
            var topics = _brokerTopology.Topics.Where(x => x.AutoDelete).Select(topic => Delete(context, topic));

            var queues = _brokerTopology.Queues.Where(x => x.AutoDelete).Select(queue => Delete(context, queue));

            await Task.WhenAll(topics.Concat(queues)).ConfigureAwait(false);
        }

        Task Declare(ClientContext context, Topic topic)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Declare topic ({0})", topic);

            return context.CreateTopic(topic.EntityName);
        }

        Task Declare(ClientContext context, QueueSubscription subscription)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Binding topic ({0}) to queue ({1})", subscription.Source, subscription.Destination);

            return context.CreateQueueSubscription(subscription.Source.EntityName, subscription.Destination.EntityName);
        }

        Task Declare(ClientContext context, Queue queue)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Declare queue ({0})", queue);

            return context.CreateQueue(queue.EntityName);
        }

        Task Delete(ClientContext context, Topic topic)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Delete topic ({0})", topic);

            return context.DeleteTopic(topic.EntityName);
        }

        Task Delete(ClientContext context, Queue queue)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Delete queue ({0})", queue);

            return context.DeleteQueue(queue.EntityName);
        }
    }
}