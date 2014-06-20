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
namespace MassTransit.Transports.RabbitMq.Pipeline
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using Logging;
    using MassTransit.Pipeline;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;


    /// <summary>
    /// Receives messages from RabbitMQ, pushing them to the InboundPipe of the service endpoint.
    /// </summary>
    public class RabbitMqBasicConsumer :
        IBasicConsumer,
        RabbitMqConsumerMetrics,
        IDisposable
    {
        readonly TaskCompletionSource<RabbitMqConsumerMetrics> _consumerComplete;
        readonly Uri _inputAddress;
        readonly ILog _log = Logger.Get<RabbitMqBasicConsumer>();
        readonly IModel _model;
        readonly ConcurrentDictionary<ulong, RabbitMqReceiveContext> _pending;
        readonly IPipe<ReceiveContext> _receivePipe;
        string _consumerTag;
        int _currentPendingDeliveryCount;
        long _deliveryCount;
        int _maxPendingDeliveryCount;
        CancellationTokenRegistration _registration;


        public RabbitMqBasicConsumer(IModel model, Uri inputAddress, IPipe<ReceiveContext> receivePipe, CancellationToken cancellationToken)
        {
            _model = model;
            _inputAddress = inputAddress;
            _receivePipe = receivePipe;
            _pending = new ConcurrentDictionary<ulong, RabbitMqReceiveContext>();

            _consumerComplete = new TaskCompletionSource<RabbitMqConsumerMetrics>();

            _registration = cancellationToken.Register(Complete);
        }

        public Task<RabbitMqConsumerMetrics> CompleteTask
        {
            get { return _consumerComplete.Task; }
        }

        public void HandleBasicConsumeOk(string consumerTag)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("ConsumerOk: {0} - {1}", _inputAddress, consumerTag);

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

            Complete();
        }

        public void HandleModelShutdown(IModel model, ShutdownEventArgs reason)
        {
            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("ModelShutdown ({0}), Max: {1}, {2}-{3}", _consumerTag, _maxPendingDeliveryCount, reason.ReplyCode,
                    reason.ReplyText);
            }

            Complete();
        }

        public async void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange,
            string routingKey,
            IBasicProperties properties, byte[] body)
        {
            Interlocked.Increment(ref _deliveryCount);

            int current = Interlocked.Increment(ref _currentPendingDeliveryCount);
            while (current > _maxPendingDeliveryCount)
                Interlocked.CompareExchange(ref _maxPendingDeliveryCount, current, _maxPendingDeliveryCount);

            try
            {
                var context = new RabbitMqReceiveContext(exchange, routingKey, _consumerTag, _inputAddress, deliveryTag,
                    body, redelivered, properties);

                if (!_pending.TryAdd(deliveryTag, context))
                {
                    // should not happen, duplicate delivery tag??
                }

                await _receivePipe.Send(context);

                Interlocked.Decrement(ref _currentPendingDeliveryCount);
                _model.BasicAck(deliveryTag, false);
            }
            catch (Exception)
            {
                Interlocked.Decrement(ref _currentPendingDeliveryCount);
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

        public void Dispose()
        {
            Complete();
        }

        public string ConsumerTag
        {
            get { return _consumerTag; }
        }

        public long DeliveryCount
        {
            get { return _deliveryCount; }
        }

        public int ConcurrentDeliveryCount
        {
            get { return _maxPendingDeliveryCount; }
        }

        void Complete()
        {
            _registration.Dispose();

            _consumerComplete.TrySetResult(this);
        }
    }
}