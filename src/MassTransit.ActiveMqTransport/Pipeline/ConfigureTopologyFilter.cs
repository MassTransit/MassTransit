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
namespace MassTransit.ActiveMqTransport.Pipeline
{
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;
    using Logging;
    using MassTransit.Pipeline;
    using Topology.Builders;
    using Topology.Entities;


    /// <summary>
    /// Configures the broker with the supplied topology once the model is created, to ensure
    /// that the exchanges, queues, and bindings for the model are properly configured in ActiveMQ.
    /// </summary>
    public class ConfigureTopologyFilter<TSettings> :
        IFilter<SessionContext>
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

        async Task IFilter<SessionContext>.Send(SessionContext context, IPipe<SessionContext> next)
        {
            await context.OneTimeSetup<ConfigureTopologyContext<TSettings>>(async payload =>
            {
                await ConfigureTopology(context).ConfigureAwait(false);

                context.GetOrAddPayload(() => _settings);
            }).ConfigureAwait(false);

            await next.Send(context).ConfigureAwait(false);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateFilterScope("configureTopology");

            _brokerTopology.Probe(scope);
        }

        async Task ConfigureTopology(SessionContext context)
        {
            await Task.WhenAll(_brokerTopology.Topics.Select(exchange => Declare(context, exchange))).ConfigureAwait(false);

            await Task.WhenAll(_brokerTopology.Queues.Select(queue => Declare(context, queue))).ConfigureAwait(false);
        }

        Task Declare(SessionContext context, Topic topic)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Declare exchange ({0})", topic);

            return context.GetTopic(topic.EntityName);
        }

        Task Declare(SessionContext context, Queue queue)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Declare queue ({0})", queue);

            return context.GetQueue(queue.EntityName);
        }
    }
}