// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.RabbitMq
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using Logging;
    using Pipeline;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;


    /// <summary>
    /// Receives messages from RabbitMQ, pushing them to the InboundPipe of the service endpoint.
    /// </summary>
    public class RabbitMqBasicConsumer :
        IBasicConsumer
    {
        readonly Uri _inputAddress;
        readonly ILog _log = Logger.Get<RabbitMqBasicConsumer>();
        readonly IModel _model;
        readonly ConcurrentDictionary<ulong, RabbitMqReceiveContext> _pending;
        readonly IPipe<ReceiveContext> _receivePipe;
        string _consumerTag;

        int _current;
        int _max;


        public RabbitMqBasicConsumer(IModel model, Uri inputAddress, IPipe<ReceiveContext> receivePipe)
        {
            _model = model;
            _inputAddress = inputAddress;
            _receivePipe = receivePipe;
            _pending = new ConcurrentDictionary<ulong, RabbitMqReceiveContext>();
        }

        public void HandleBasicConsumeOk(string consumerTag)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("ConsumerOk: {0}", consumerTag);

            _consumerTag = consumerTag;
        }

        public void HandleBasicCancelOk(string consumerTag)
        {
        }

        public void HandleBasicCancel(string consumerTag)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Consumer Cancelled: {0}", consumerTag);

            foreach (RabbitMqReceiveContext context in _pending.Values)
                context.Cancel();

            if (ConsumerCancelled != null)
                ConsumerCancelled(this, new ConsumerEventArgs(consumerTag));
        }

        public void HandleModelShutdown(IModel model, ShutdownEventArgs reason)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("ModelShutdown ({0}), Max: {1}, {2}-{3}", _consumerTag, _max, reason.ReplyCode, reason.ReplyText);
        }

        public async void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange,
            string routingKey,
            IBasicProperties properties, byte[] body)
        {
            int current = Interlocked.Increment(ref _current);
            while (current > _max)
                Interlocked.CompareExchange(ref _max, current, _max);

            try
            {
                var context = new RabbitMqReceiveContext(exchange, routingKey, _consumerTag, _inputAddress, deliveryTag,
                    body, redelivered, properties);

                if (!_pending.TryAdd(deliveryTag, context))
                {
                    // should not happen, duplicate delivery tag??
                }

                await _receivePipe.Send(context);

                Interlocked.Decrement(ref _current);
                _model.BasicAck(deliveryTag, false);
            }
            catch (Exception)
            {
                Interlocked.Decrement(ref _current);
                _model.BasicNack(deliveryTag, false, true);
            }
            finally
            {
                RabbitMqReceiveContext ignored;
                _pending.TryRemove(deliveryTag, out ignored);
            }
        }

        public IModel Model
        {
            get { return _model; }
        }

        public event ConsumerCancelledEventHandler ConsumerCancelled;
    }
}