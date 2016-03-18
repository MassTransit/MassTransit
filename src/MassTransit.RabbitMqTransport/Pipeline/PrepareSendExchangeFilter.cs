// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Logging;
    using MassTransit.Pipeline;
    using RabbitMQ.Client;
    using Topology;


    /// <summary>
    /// Prepares a queue for receiving messages using the ReceiveSettings specified.
    /// </summary>
    public class PrepareSendExchangeFilter :
        IFilter<ModelContext>
    {
        readonly ExchangeBindingSettings[] _exchangeBindings;
        readonly ILog _log = Logger.Get<PrepareSendExchangeFilter>();
        readonly SendSettings _settings;

        public PrepareSendExchangeFilter(SendSettings settings, params ExchangeBindingSettings[] exchangeBindings)
        {
            _settings = settings;
            _exchangeBindings = exchangeBindings.Concat(settings.ExchangeBindings).ToArray();
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        async Task IFilter<ModelContext>.Send(ModelContext context, IPipe<ModelContext> next)
        {
            if (IsFirstTime(context))
            {
                await DeclareExchange(context).ConfigureAwait(false);

                if (_settings.BindToQueue)
                    await DeclareAndBindQueue(context).ConfigureAwait(false);

                for (int i = 0; i < _exchangeBindings.Length; i++)
                {
                    ExchangeBindingSettings binding = _exchangeBindings[i];

                    ExchangeSettings exchange = binding.Exchange;

                    if (_log.IsDebugEnabled)
                    {
                        _log.DebugFormat("Exchange: {0} ({1})", exchange.ExchangeName,
                            string.Join(", ", new[]
                            {
                                exchange.Durable ? "durable" : "",
                                exchange.AutoDelete ? "auto-delete" : ""
                            }.Where(x => !string.IsNullOrWhiteSpace(x))));
                    }

                    await context.ExchangeDeclare(exchange.ExchangeName, exchange.ExchangeType, exchange.Durable, exchange.AutoDelete,
                        exchange.Arguments).ConfigureAwait(false);

                    await context.ExchangeBind(exchange.ExchangeName, _settings.ExchangeName, binding.RoutingKey, new Dictionary<string, object>()).ConfigureAwait(false);

                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Exchange:Exchange Binding: {0} ({1})", exchange.ExchangeName, _settings.ExchangeName);
                }
            }

            await next.Send(context).ConfigureAwait(false);
        }

        bool IsFirstTime(ModelContext context)
        {
            bool added = false;
            context.GetOrAddPayload(() =>
            {
                added = true;
                return _settings;
            });
            return added;
        }

        async Task DeclareExchange(ModelContext context)
        {
            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Exchange: {0} ({1})", _settings.ExchangeName, _settings);
            }

            if (!string.IsNullOrWhiteSpace(_settings.ExchangeName))
            {
                await context.ExchangeDeclare(_settings.ExchangeName, _settings.ExchangeType, _settings.Durable, _settings.AutoDelete,
                    _settings.ExchangeArguments).ConfigureAwait(false);
            }

            context.GetOrAddPayload(() => _settings);
        }

        async Task DeclareAndBindQueue(ModelContext context)
        {
            QueueDeclareOk queueOk = await context.QueueDeclare(_settings.QueueName, _settings.Durable, false,
                _settings.AutoDelete, _settings.QueueArguments).ConfigureAwait(false);

            string queueName = queueOk.QueueName;

            await context.QueueBind(queueName, _settings.ExchangeName, "", new Dictionary<string, object>()).ConfigureAwait(false);

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Exchange:Queue Binding: {0} ({1})", _settings.ExchangeName, queueName);
        }
    }
}