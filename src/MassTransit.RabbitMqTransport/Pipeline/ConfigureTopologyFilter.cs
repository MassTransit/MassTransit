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
namespace MassTransit.RabbitMqTransport.Pipeline
{
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;
    using Logging;
    using MassTransit.Pipeline;
    using Topology;
    using Topology.Builders;
    using Topology.Entities;


    /// <summary>
    /// Configures the broker with the supplied topology once the model is created, to ensure
    /// that the exchanges, queues, and bindings for the model are properly configured in RabbitMQ.
    /// </summary>
    public class ConfigureTopologyFilter<TSettings> :
        IFilter<ModelContext>
        where TSettings : class
    {
        readonly ILog _log = Logger.Get<ConfigureTopologyFilter<TSettings>>();
        readonly TSettings _settings;
        readonly BrokerTopology _brokerTopology;

        public ConfigureTopologyFilter(TSettings settings, BrokerTopology brokerTopology)
        {
            _settings = settings;
            _brokerTopology = brokerTopology;
        }

        async Task IFilter<ModelContext>.Send(ModelContext context, IPipe<ModelContext> next)
        {
            await context.OneTimeSetup<ConfigureTopologyContext>(async payload =>
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

        async Task ConfigureTopology(ModelContext context)
        {
            await Task.WhenAll(_brokerTopology.Exchanges.Select(exchange => Declare(context, exchange))).ConfigureAwait(false);

            await Task.WhenAll(_brokerTopology.ExchangeBindings.Select(binding => Bind(context, binding))).ConfigureAwait(false);

            await Task.WhenAll(_brokerTopology.Queues.Select(queue => Declare(context, queue))).ConfigureAwait(false);

            await Task.WhenAll(_brokerTopology.QueueBindings.Select(binding => Bind(context, binding))).ConfigureAwait(false);
        }

        Task Declare(ModelContext context, Exchange exchange)
        {
            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Declare exchange ({0})", exchange);
            }

            return context.ExchangeDeclare(exchange.ExchangeName, exchange.ExchangeType, exchange.Durable, exchange.AutoDelete, exchange.ExchangeArguments);
        }

        Task Declare(ModelContext context, Queue queue)
        {
            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Declare queue ({0})", queue);
            }

            return context.QueueDeclare(queue.QueueName, queue.Durable, queue.Exclusive, queue.AutoDelete, queue.QueueArguments);
        }

        Task Bind(ModelContext context, ExchangeToExchangeBinding binding)
        {
            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Bind exchange to exchange ({0})", binding);
            }

            return context.ExchangeBind(binding.Destination.ExchangeName, binding.Source.ExchangeName, binding.RoutingKey, binding.Arguments);
        }

        Task Bind(ModelContext context, ExchangeToQueueBinding binding)
        {
            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Bind exchange to queue ({0})", binding);
            }

            return context.QueueBind(binding.Destination.QueueName, binding.Source.ExchangeName, binding.RoutingKey, binding.Arguments);
        }


        public interface ConfigureTopologyContext
        {
        }
    }
}