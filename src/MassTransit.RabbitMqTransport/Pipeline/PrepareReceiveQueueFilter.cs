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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Logging;
    using MassTransit.Pipeline;
    using RabbitMQ.Client;
    using Topology;
    using Util;


    public interface ISetPrefetchCount
    {
        Task SetPrefetchCount(ushort prefetchCount);
    }


    /// <summary>
    /// Prepares a queue for receiving messages using the ReceiveSettings specified.
    /// </summary>
    public class PrepareReceiveQueueFilter :
        IFilter<ModelContext>
    {
        static readonly ILog _log = Logger.Get<PrepareReceiveQueueFilter>();
        readonly ExchangeBindingSettings[] _exchangeBindings;
        readonly Mediator<ISetPrefetchCount> _prefetchCountMediator;
        readonly ReceiveSettings _settings;
        bool _queueAlreadyPurged;

        public PrepareReceiveQueueFilter(ReceiveSettings settings, Mediator<ISetPrefetchCount> prefetchCountMediator,
            params ExchangeBindingSettings[] exchangeBindings)
        {
            _settings = settings;
            _prefetchCountMediator = prefetchCountMediator;
            _exchangeBindings = exchangeBindings;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        async Task IFilter<ModelContext>.Send(ModelContext context, IPipe<ModelContext> next)
        {
            await context.BasicQos(0, _settings.PrefetchCount, false).ConfigureAwait(false);

            QueueDeclareOk queueOk = await context.QueueDeclare(_settings.QueueName, _settings.Durable, _settings.Exclusive,
                _settings.AutoDelete, _settings.QueueArguments).ConfigureAwait(false);

            string queueName = queueOk.QueueName;

            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Queue: {0} ({1})", queueName,
                    string.Join(", ", new[]
                    {
                        _settings.Durable ? "durable" : "",
                        _settings.Exclusive ? "exclusive" : "",
                        _settings.AutoDelete ? "auto-delete" : ""
                    }.Where(x => !string.IsNullOrWhiteSpace(x))));
            }

            if (_settings.PurgeOnStartup)
                await PurgeIfRequested(context, queueOk, queueName).ConfigureAwait(false);

            string exchangeName = _settings.ExchangeName ?? queueName;

            if (!string.IsNullOrWhiteSpace(_settings.ExchangeName) || string.IsNullOrWhiteSpace(_settings.QueueName))
            {
                await context.ExchangeDeclare(exchangeName, _settings.ExchangeType, _settings.Durable, _settings.AutoDelete,
                    _settings.ExchangeArguments).ConfigureAwait(false);

                await context.QueueBind(queueName, exchangeName, "", new Dictionary<string, object>()).ConfigureAwait(false);
            }

            await ApplyExchangeBindings(context, exchangeName).ConfigureAwait(false);

            ReceiveSettings settings = new RabbitMqReceiveSettings(_settings)
            {
                QueueName = queueName,
                ExchangeName = exchangeName
            };

            context.GetOrAddPayload(() => settings);

            using (new SetPrefetchCount(_prefetchCountMediator, context))
            {
                await next.Send(context).ConfigureAwait(false);
            }
        }

        Task ApplyExchangeBindings(ModelContext context, string exchangeName)
        {
            return Task.WhenAll(_exchangeBindings.Select(async binding =>
            {
                ExchangeSettings exchange = binding.Exchange;

                await context.ExchangeDeclare(exchange.ExchangeName, exchange.ExchangeType, exchange.Durable, exchange.AutoDelete,
                    exchange.Arguments).ConfigureAwait(false);

                await context.ExchangeBind(exchangeName, exchange.ExchangeName, binding.RoutingKey, binding.Arguments).ConfigureAwait(false);
            }));
        }

        async Task PurgeIfRequested(ModelContext context, QueueDeclareOk queueOk, string queueName)
        {
            if (!_queueAlreadyPurged)
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Purging {0} messages from queue {1}", queueOk.MessageCount, queueName);

                var purgedMessageCount = await context.QueuePurge(queueName).ConfigureAwait(false);

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Purged {0} messages from queue {1}", purgedMessageCount, queueName);

                _queueAlreadyPurged = true;
            }
            else
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Queue {0} already purged at startup, skipping", queueName);
            }
        }


        class SetPrefetchCount :
            ISetPrefetchCount,
            IDisposable
        {
            readonly ConnectHandle _handle;
            readonly ModelContext _modelContext;

            public SetPrefetchCount(Mediator<ISetPrefetchCount> mediator, ModelContext modelContext)
            {
                _modelContext = modelContext;

                _handle = mediator.Connect(this);
            }

            public void Dispose()
            {
                _handle.Dispose();
            }

            async Task ISetPrefetchCount.SetPrefetchCount(ushort prefetchCount)
            {
                await _modelContext.BasicQos(0, prefetchCount, false).ConfigureAwait(false);
            }
        }
    }
}