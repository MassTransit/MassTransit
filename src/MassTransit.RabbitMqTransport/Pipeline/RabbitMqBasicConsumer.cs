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
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using Internals.Extensions;
    using Logging;
    using MassTransit.Pipeline;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using Util;


    /// <summary>
    /// Receives messages from RabbitMQ, pushing them to the InboundPipe of the service endpoint.
    /// </summary>
    public class RabbitMqBasicConsumer :
        IBasicConsumer,
        RabbitMqConsumerMetrics
    {
        readonly Uri _inputAddress;
        readonly ILog _log = Logger.Get<RabbitMqBasicConsumer>();
        readonly ModelContext _model;
        readonly ITaskParticipant _participant;
        readonly ConcurrentDictionary<ulong, RabbitMqReceiveContext> _pending;
        readonly IReceiveObserver _receiveObserver;
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly ReceiveSettings _receiveSettings;
        string _consumerTag;
        int _currentPendingDeliveryCount;
        long _deliveryCount;
        int _maxPendingDeliveryCount;
        bool _shuttingDown;
        readonly TaskCompletionSource<bool> _deliveryComplete;

        /// <summary>
        /// The basic consumer receives messages pushed from the broker.
        /// </summary>
        /// <param name="model">The model context for the consumer</param>
        /// <param name="inputAddress">The input address for messages received by the consumer</param>
        /// <param name="receivePipe">The receive pipe to dispatch messages</param>
        /// <param name="receiveObserver">The observer for receive events</param>
        /// <param name="taskSupervisor">The token used to cancel/stop the consumer at shutdown</param>
        public RabbitMqBasicConsumer(ModelContext model, Uri inputAddress, IPipe<ReceiveContext> receivePipe, IReceiveObserver receiveObserver,
            ITaskScope taskSupervisor)
        {
            _model = model;
            _inputAddress = inputAddress;
            _receivePipe = receivePipe;
            _receiveObserver = receiveObserver;

            _receiveSettings = model.GetPayload<ReceiveSettings>();

            _pending = new ConcurrentDictionary<ulong, RabbitMqReceiveContext>();

            _participant = taskSupervisor.CreateParticipant($"{TypeMetadataCache<RabbitMqBasicConsumer>.ShortName} - {inputAddress}", Stop);
            _deliveryComplete = new TaskCompletionSource<bool>();
        }

        /// <summary>
        /// Called when the consumer is ready to be delivered messages by the broker
        /// </summary>
        /// <param name="consumerTag"></param>
        void IBasicConsumer.HandleBasicConsumeOk(string consumerTag)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("ConsumerOk: {0} - {1}", _inputAddress, consumerTag);

            _consumerTag = consumerTag;

            _participant.SetReady();
        }

        /// <summary>
        /// Called when the broker has received and acknowledged the BasicCancel, indicating
        /// that the consumer is requesting to be shut down gracefully.
        /// </summary>
        /// <param name="consumerTag">The consumerTag that was shut down.</param>
        void IBasicConsumer.HandleBasicCancelOk(string consumerTag)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Consumer Cancel Ok: {0} - {1}", _inputAddress, consumerTag);

            _deliveryComplete.TrySetResult(true);
            _participant.SetComplete();
        }

        /// <summary>
        /// Called when the broker cancels the consumer due to an unexpected event, such as a
        /// queue removal, or other change, that would disconnect the consumer.
        /// </summary>
        /// <param name="consumerTag">The consumerTag that is being cancelled.</param>
        void IBasicConsumer.HandleBasicCancel(string consumerTag)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Consumer Cancelled: {0}", consumerTag);

            foreach (RabbitMqReceiveContext context in _pending.Values)
                context.Cancel();

            ConsumerCancelled?.Invoke(this, new ConsumerEventArgs(consumerTag));

            _deliveryComplete.TrySetResult(true);
            _participant.SetComplete();
        }

        void IBasicConsumer.HandleModelShutdown(object model, ShutdownEventArgs reason)
        {
            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Consumer Model Shutdown ({0}), Concurrent Peak: {1}, {2}-{3}", _consumerTag, _maxPendingDeliveryCount, reason.ReplyCode,
                    reason.ReplyText);
            }

            _deliveryComplete.TrySetResult(false);
            _participant.SetComplete();
        }

        async void IBasicConsumer.HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange,
            string routingKey,
            IBasicProperties properties, byte[] body)
        {
            if (_shuttingDown)
            {
                await WaitAndAbandonMessage(deliveryTag).ConfigureAwait(false);
                return;
            }

            Interlocked.Increment(ref _deliveryCount);

            int current = Interlocked.Increment(ref _currentPendingDeliveryCount);
            while (current > _maxPendingDeliveryCount)
                Interlocked.CompareExchange(ref _maxPendingDeliveryCount, current, _maxPendingDeliveryCount);

            var context = new RabbitMqReceiveContext(_inputAddress, exchange, routingKey, _consumerTag, deliveryTag, body, redelivered, properties,
                _receiveObserver);

            context.GetOrAddPayload(() => _receiveSettings);
            context.GetOrAddPayload(() => _model);
            context.GetOrAddPayload(() => _model.ConnectionContext);

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
                try
                {
                    _model.BasicNack(deliveryTag, false, true);
                }
                catch (Exception ackEx)
                {
                    if (_log.IsErrorEnabled)
                        _log.ErrorFormat("An error occurred trying to NACK a message with delivery tag {0}: {1}", deliveryTag, ackEx.ToString());
                }
            }
            finally
            {
                RabbitMqReceiveContext ignored;
                _pending.TryRemove(deliveryTag, out ignored);

                int pendingCount = Interlocked.Decrement(ref _currentPendingDeliveryCount);
                if (pendingCount == 0 && _shuttingDown)
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Consumer shutdown completed: {0}", _inputAddress);

                    _deliveryComplete.TrySetResult(true);
                }
            }
        }

        IModel IBasicConsumer.Model => _model.Model;

        public event EventHandler<ConsumerEventArgs> ConsumerCancelled;

        string RabbitMqConsumerMetrics.ConsumerTag => _consumerTag;

        long RabbitMqConsumerMetrics.DeliveryCount => _deliveryCount;

        int RabbitMqConsumerMetrics.ConcurrentDeliveryCount => _maxPendingDeliveryCount;

        async Task WaitAndAbandonMessage(ulong deliveryTag)
        {
            try
            {
                await _deliveryComplete.Task.ConfigureAwait(false);

                _model.BasicNack(deliveryTag, false, true);
            }
            catch (Exception exception)
            {
                if (_log.IsErrorEnabled)
                    _log.Debug("Shutting down, nack message faulted: {_inputAddress}", exception);
            }
        }

        async Task Stop()
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Shutting down consumer: {0}", _inputAddress);

            _shuttingDown = true;

            if (_currentPendingDeliveryCount > 0)
            {
                try
                {
                    using (var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(60)))
                    {
                        await _deliveryComplete.Task.WithCancellation(cancellation.Token).ConfigureAwait(false);
                    }
                }
                catch (TaskCanceledException)
                {
                    if (_log.IsWarnEnabled)
                        _log.WarnFormat("Timeout waiting for consumer to exit: {0}", _inputAddress);
                }
            }

            await _model.BasicCancel(_consumerTag).ConfigureAwait(false);

            await _participant.ParticipantCompleted.ConfigureAwait(false);
        }
    }
}