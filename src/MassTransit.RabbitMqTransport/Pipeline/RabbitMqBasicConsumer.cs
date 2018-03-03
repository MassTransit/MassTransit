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
namespace MassTransit.RabbitMqTransport.Pipeline
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Logging;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using Topology;
    using Transports;
    using Transports.Metrics;
    using Util;


    /// <summary>
    /// Receives messages from RabbitMQ, pushing them to the InboundPipe of the service endpoint.
    /// </summary>
    public class RabbitMqBasicConsumer :
        Supervisor,
        IBasicConsumer,
        RabbitMqDeliveryMetrics
    {
        readonly IDeadLetterTransport _deadLetterTransport;
        readonly TaskCompletionSource<bool> _deliveryComplete;
        readonly IErrorTransport _errorTransport;
        readonly Uri _inputAddress;
        readonly ILog _log = Logger.Get<RabbitMqBasicConsumer>();
        readonly ModelContext _model;
        readonly ConcurrentDictionary<ulong, RabbitMqReceiveContext> _pending;
        readonly IReceiveObserver _receiveObserver;
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly ReceiveSettings _receiveSettings;
        readonly RabbitMqReceiveEndpointContext _receiveEndpointContext;
        readonly IDeliveryTracker _tracker;

        string _consumerTag;

        /// <summary>
        /// The basic consumer receives messages pushed from the broker.
        /// </summary>
        /// <param name="model">The model context for the consumer</param>
        /// <param name="inputAddress">The input address for messages received by the consumer</param>
        /// <param name="receivePipe">The receive pipe to dispatch messages</param>
        /// <param name="receiveObserver">The observer for receive events</param>
        /// <param name="receiveEndpointContext">The topology</param>
        /// <param name="deadLetterTransport"></param>
        /// <param name="errorTransport"></param>
        public RabbitMqBasicConsumer(ModelContext model, Uri inputAddress, IPipe<ReceiveContext> receivePipe, IReceiveObserver receiveObserver,
            RabbitMqReceiveEndpointContext receiveEndpointContext, IDeadLetterTransport deadLetterTransport, IErrorTransport errorTransport)
        {
            _model = model;
            _inputAddress = inputAddress;
            _receivePipe = receivePipe;
            _receiveObserver = receiveObserver;
            _receiveEndpointContext = receiveEndpointContext;
            _deadLetterTransport = deadLetterTransport;
            _errorTransport = errorTransport;

            _tracker = new DeliveryTracker(HandleDeliveryComplete);

            _receiveSettings = model.GetPayload<ReceiveSettings>();

            _pending = new ConcurrentDictionary<ulong, RabbitMqReceiveContext>();

            _deliveryComplete = new TaskCompletionSource<bool>();
        }

        /// <summary>
        /// Called when the consumer is ready to be delivered messages by the broker
        /// </summary>
        /// <param name="consumerTag"></param>
        void IBasicConsumer.HandleBasicConsumeOk(string consumerTag)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("ConsumerOk: {0} - {1}", _receiveEndpointContext.InputAddress, consumerTag);

            _consumerTag = consumerTag;

            SetReady();
        }

        /// <summary>
        /// Called when the broker has received and acknowledged the BasicCancel, indicating
        /// that the consumer is requesting to be shut down gracefully.
        /// </summary>
        /// <param name="consumerTag">The consumerTag that was shut down.</param>
        void IBasicConsumer.HandleBasicCancelOk(string consumerTag)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Consumer Cancel Ok: {0} - {1}", _receiveEndpointContext.InputAddress, consumerTag);

            _deliveryComplete.TrySetResult(true);
            SetCompleted(TaskUtil.Completed);
        }

        /// <summary>
        /// Called when the broker cancels the consumer due to an unexpected event, such as a
        /// queue removal, or other change, that would disconnect the consumer.
        /// </summary>
        /// <param name="consumerTag">The consumerTag that is being cancelled.</param>
        void IBasicConsumer.HandleBasicCancel(string consumerTag)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Consumer Canceled: {0}", consumerTag);

            foreach (var context in _pending.Values)
                context.Cancel();

            ConsumerCancelled?.Invoke(this, new ConsumerEventArgs(consumerTag));

            _deliveryComplete.TrySetResult(true);
            SetCompleted(TaskUtil.Completed);
        }

        void IBasicConsumer.HandleModelShutdown(object model, ShutdownEventArgs reason)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Consumer Model Shutdown ({0}), Concurrent Peak: {1}, {2}-{3}", _consumerTag, _tracker.MaxConcurrentDeliveryCount,
                    reason.ReplyCode,
                    reason.ReplyText);

            _deliveryComplete.TrySetResult(false);
            SetCompleted(TaskUtil.Completed);
        }

        async void IBasicConsumer.HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange,
            string routingKey,
            IBasicProperties properties, byte[] body)
        {
            if (IsStopping)
            {
                await WaitAndAbandonMessage(deliveryTag).ConfigureAwait(false);
                return;
            }

            using (var delivery = _tracker.BeginDelivery())
            {
                var context = new RabbitMqReceiveContext(_inputAddress, exchange, routingKey, _consumerTag, deliveryTag, body, redelivered, properties,
                    _receiveObserver, _receiveEndpointContext);

                context.GetOrAddPayload(() => _errorTransport);
                context.GetOrAddPayload(() => _deadLetterTransport);

                context.GetOrAddPayload(() => _receiveSettings);
                context.GetOrAddPayload(() => _model);
                context.GetOrAddPayload(() => _model.ConnectionContext);

                try
                {
                    if (!_pending.TryAdd(deliveryTag, context))
                        if (_log.IsErrorEnabled)
                            _log.ErrorFormat("Duplicate BasicDeliver: {0}", deliveryTag);

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
                    _pending.TryRemove(deliveryTag, out _);

                    context.Dispose();
                }
            }
        }

        IModel IBasicConsumer.Model => _model.Model;

        public event EventHandler<ConsumerEventArgs> ConsumerCancelled;

        string RabbitMqDeliveryMetrics.ConsumerTag => _consumerTag;

        long DeliveryMetrics.DeliveryCount => _tracker.DeliveryCount;

        int DeliveryMetrics.ConcurrentDeliveryCount => _tracker.MaxConcurrentDeliveryCount;

        void HandleDeliveryComplete()
        {
            if (IsStopping)
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Consumer shutdown completed: {0}", _receiveEndpointContext.InputAddress);

                _deliveryComplete.TrySetResult(true);
            }
        }

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
                    _log.Debug("Shutting down, nack message faulted: {_topology.InputAddress}", exception);
            }
        }

        protected override async Task StopSupervisor(StopSupervisorContext context)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Stopping consumer: {0}", _receiveEndpointContext.InputAddress);

            SetCompleted(ActiveAndActualAgentsCompleted(context));

            await Completed.ConfigureAwait(false);
        }

        async Task ActiveAndActualAgentsCompleted(StopSupervisorContext context)
        {
            await Task.WhenAll(context.Agents.Select(x => Completed)).UntilCompletedOrCanceled(context.CancellationToken).ConfigureAwait(false);

            if (_tracker.ActiveDeliveryCount > 0)
            {
                try
                {
                    await _deliveryComplete.Task.UntilCompletedOrCanceled(context.CancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    if (_log.IsWarnEnabled)
                        _log.WarnFormat("Stop canceled waiting for message consumers to complete: {0}", _receiveEndpointContext.InputAddress);
                }
            }

            try
            {
                await _model.BasicCancel(_consumerTag).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                if (_log.IsWarnEnabled)
                    _log.WarnFormat("Exception canceling the consumer: {0}", _receiveEndpointContext.InputAddress);
            }
        }
    }
}