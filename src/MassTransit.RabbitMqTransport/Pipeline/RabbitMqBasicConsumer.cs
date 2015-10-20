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
        readonly ModelContext _model;
        readonly ConcurrentDictionary<ulong, RabbitMqReceiveContext> _pending;
        readonly IReceiveObserver _receiveObserver;
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly ReceiveSettings _receiveSettings;
        string _consumerTag;
        int _currentPendingDeliveryCount;
        long _deliveryCount;
        int _maxPendingDeliveryCount;
        CancellationTokenRegistration _registration;

        public RabbitMqBasicConsumer(ModelContext model, Uri inputAddress, IPipe<ReceiveContext> receivePipe, IReceiveObserver receiveObserver,
            CancellationToken cancellationToken)
        {
            _model = model;
            _inputAddress = inputAddress;
            _receivePipe = receivePipe;
            _receiveObserver = receiveObserver;

            _receiveSettings = model.GetPayload<ReceiveSettings>();

            _pending = new ConcurrentDictionary<ulong, RabbitMqReceiveContext>();

            _consumerComplete = new TaskCompletionSource<RabbitMqConsumerMetrics>();

            _registration = cancellationToken.Register(Complete);
        }

        public Task<RabbitMqConsumerMetrics> CompleteTask => _consumerComplete.Task;

        void IBasicConsumer.HandleBasicConsumeOk(string consumerTag)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("ConsumerOk: {0} - {1}", _inputAddress, consumerTag);

            _consumerTag = consumerTag;
        }

        void IBasicConsumer.HandleBasicCancelOk(string consumerTag)
        {
        }

        void IBasicConsumer.HandleBasicCancel(string consumerTag)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Consumer Cancelled: {0}", consumerTag);

            foreach (RabbitMqReceiveContext context in _pending.Values)
                context.Cancel();

            ConsumerCancelled?.Invoke(this, new ConsumerEventArgs(consumerTag));

            Complete();
        }

        void IBasicConsumer.HandleModelShutdown(object model, ShutdownEventArgs reason)
        {
            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Consumer Model Shutdown ({0}), Concurrent Peak: {1}, {2}-{3}", _consumerTag, _maxPendingDeliveryCount, reason.ReplyCode,
                    reason.ReplyText);
            }

            Complete();
        }

        async void IBasicConsumer.HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange,
            string routingKey,
            IBasicProperties properties, byte[] body)
        {
            Interlocked.Increment(ref _deliveryCount);

            int current = Interlocked.Increment(ref _currentPendingDeliveryCount);
            while (current > _maxPendingDeliveryCount)
                Interlocked.CompareExchange(ref _maxPendingDeliveryCount, current, _maxPendingDeliveryCount);

            await Task.Yield();

            var context = new RabbitMqReceiveContext(_inputAddress, exchange, routingKey, _consumerTag, deliveryTag, body, redelivered, properties,
                _receiveObserver);

            context.GetOrAddPayload(() => _receiveSettings);
            context.GetOrAddPayload(() => _model);

            try
            {
                if (!_pending.TryAdd(deliveryTag, context))
                {
                    if (_log.IsErrorEnabled)
                        _log.ErrorFormat("Duplicate BasicDeliver: {0}", deliveryTag);
                }

                await _receiveObserver.PreReceive(context).ConfigureAwait(false);

                await _receivePipe.Send(context).ConfigureAwait(false);

                await context.CompleteTask.ConfigureAwait(false);

                _model.BasicAck(deliveryTag, false);

                await _receiveObserver.PostReceive(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _receiveObserver.ReceiveFault(context, ex).ConfigureAwait(false);

                _model.BasicNack(deliveryTag, false, true);
            }
            finally
            {
                Interlocked.Decrement(ref _currentPendingDeliveryCount);

                RabbitMqReceiveContext ignored;
                _pending.TryRemove(deliveryTag, out ignored);
            }
        }

        IModel IBasicConsumer.Model => _model.Model;

        public event EventHandler<ConsumerEventArgs> ConsumerCancelled;

        void IDisposable.Dispose()
        {
            _registration.Dispose();

            Complete();
        }

        string RabbitMqConsumerMetrics.ConsumerTag => _consumerTag;

        long RabbitMqConsumerMetrics.DeliveryCount => _deliveryCount;

        int RabbitMqConsumerMetrics.ConcurrentDeliveryCount => _maxPendingDeliveryCount;

        void Complete()
        {
            _consumerComplete.TrySetResult(this);
        }
    }
}