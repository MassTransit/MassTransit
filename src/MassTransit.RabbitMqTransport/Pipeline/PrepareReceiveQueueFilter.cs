// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Contracts;
    using Logging;
    using Management;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Pipes;
    using RabbitMQ.Client;
    using Topology;
    using Util;


    /// <summary>
    /// Prepares a queue for receiving messages using the ReceiveSettings specified.
    /// </summary>
    public class PrepareReceiveQueueFilter :
        IFilter<ModelContext>,
        ISetPrefetchCount
    {
        static readonly ILog _log = Logger.Get<PrepareReceiveQueueFilter>();
        readonly ExchangeBindingSettings[] _exchangeBindings;
        readonly IManagementPipe _managementPipe;
        readonly ReceiveSettings _settings;
        ushort _prefetchCount;
        bool _queueAlreadyPurged;

        public PrepareReceiveQueueFilter(ReceiveSettings settings, IManagementPipe managementPipe, params ExchangeBindingSettings[] exchangeBindings)
        {
            _settings = settings;
            _prefetchCount = settings.PrefetchCount;
            _managementPipe = managementPipe;
            _exchangeBindings = exchangeBindings;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
        }

        async Task IFilter<ModelContext>.Send(ModelContext context, IPipe<ModelContext> next)
        {
            await context.BasicQos(0, _prefetchCount, true).ConfigureAwait(false);

            var queueOk = await context.QueueDeclare(_settings.QueueName, _settings.Durable, _settings.Exclusive,
                _settings.AutoDelete, _settings.QueueArguments).ConfigureAwait(false);

            var queueName = queueOk.QueueName;

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

            var exchangeName = _settings.ExchangeName ?? queueName;

            if (!string.IsNullOrWhiteSpace(exchangeName))
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

            using (new SetModelPrefetchCountConsumer(_managementPipe, context, this))
            {
                await next.Send(context).ConfigureAwait(false);
            }
        }

        public Task SetPrefetchCount(ushort prefetchCount)
        {
            _prefetchCount = prefetchCount;

            return TaskUtil.Completed;
        }

        Task ApplyExchangeBindings(ModelContext context, string exchangeName)
        {
            return Task.WhenAll(_exchangeBindings.Select(async binding =>
            {
                var exchange = binding.Exchange;

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


        class SetModelPrefetchCountConsumer :
            IConsumer<SetPrefetchCount>,
            IDisposable
        {
            readonly ISetPrefetchCount _filter;
            readonly ConnectHandle _handle;
            readonly ModelContext _modelContext;

            public SetModelPrefetchCountConsumer(IManagementPipe managementPipe, ModelContext modelContext, ISetPrefetchCount filter)
            {
                _modelContext = modelContext;
                _filter = filter;

                _handle = managementPipe.ConnectInstance(this);
            }

            async Task IConsumer<SetPrefetchCount>.Consume(ConsumeContext<SetPrefetchCount> context)
            {
                var prefetchCount = context.Message.PrefetchCount;

                await _modelContext.BasicQos(0, prefetchCount, true).ConfigureAwait(false);

                await _filter.SetPrefetchCount(prefetchCount).ConfigureAwait(false);
            }

            public void Dispose()
            {
                _handle.Dispose();
            }
        }
    }
}