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
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.NMS;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Logging;
    using Topology;
    using Transports.Metrics;


    /// <summary>
    /// Receives messages from ActiveMQ, pushing them to the InboundPipe of the service endpoint.
    /// </summary>
    public sealed class ActiveMqBasicConsumer :
        Supervisor,
        DeliveryMetrics
    {
        readonly TaskCompletionSource<bool> _deliveryComplete;
        readonly Uri _inputAddress;
        readonly ILog _log = Logger.Get<ActiveMqBasicConsumer>();
        readonly SessionContext _session;
        readonly IMessageConsumer _messageConsumer;
        readonly ConcurrentDictionary<string, ActiveMqReceiveContext> _pending;
        readonly ReceiveSettings _receiveSettings;
        readonly ActiveMqReceiveEndpointContext _context;
        readonly IDeliveryTracker _tracker;

        /// <summary>
        /// The basic consumer receives messages pushed from the broker.
        /// </summary>
        /// <param name="session">The model context for the consumer</param>
        /// <param name="messageConsumer"></param>
        /// <param name="inputAddress">The input address for messages received by the consumer</param>
        /// <param name="context">The topology</param>
        public ActiveMqBasicConsumer(SessionContext session, IMessageConsumer messageConsumer, Uri inputAddress, ActiveMqReceiveEndpointContext context)
        {
            _session = session;
            _messageConsumer = messageConsumer;
            _inputAddress = inputAddress;
            _context = context;

            _tracker = new DeliveryTracker(HandleDeliveryComplete);

            _receiveSettings = session.GetPayload<ReceiveSettings>();

            _pending = new ConcurrentDictionary<string, ActiveMqReceiveContext>();

            _deliveryComplete = new TaskCompletionSource<bool>();

            messageConsumer.Listener += HandleMessage;

            SetReady();
        }

        async void HandleMessage(IMessage message)
        {
            if (IsStopping)
            {
                await WaitAndAbandonMessage(message).ConfigureAwait(false);
                return;
            }

            using (var delivery = _tracker.BeginDelivery())
            {
                var context = new ActiveMqReceiveContext(_inputAddress, message, _context);

                context.GetOrAddPayload(() => _receiveSettings);
                context.GetOrAddPayload(() => _session);
                context.GetOrAddPayload(() => _session.ConnectionContext);

                try
                {
                    if (!_pending.TryAdd(message.NMSMessageId, context))
                        if (_log.IsErrorEnabled)
                            _log.ErrorFormat("Duplicate BasicDeliver: {0}", message.NMSMessageId);

                    await _context.ReceiveObservers.PreReceive(context).ConfigureAwait(false);

                    await _context.ReceivePipe.Send(context).ConfigureAwait(false);

                    await context.ReceiveCompleted.ConfigureAwait(false);

                    message.Acknowledge();

                    await _context.ReceiveObservers.PostReceive(context).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await _context.ReceiveObservers.ReceiveFault(context, ex).ConfigureAwait(false);
                    try
                    {
                        //                        _session.BasicNack(deliveryTag, false, true);
                    }
                    catch (Exception ackEx)
                    {
                        if (_log.IsErrorEnabled)
                            _log.ErrorFormat("An error occurred trying to NACK a message with delivery tag {0}: {1}", message.NMSMessageId, ackEx.ToString());
                    }
                }
                finally
                {
                    _pending.TryRemove(message.NMSMessageId, out _);

                    context.Dispose();
                }
            }
        }

        long DeliveryMetrics.DeliveryCount => _tracker.DeliveryCount;

        int DeliveryMetrics.ConcurrentDeliveryCount => _tracker.MaxConcurrentDeliveryCount;

        void HandleDeliveryComplete()
        {
            if (IsStopping)
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Consumer shutdown completed: {0}", _context.InputAddress);

                _deliveryComplete.TrySetResult(true);
            }
        }

        async Task WaitAndAbandonMessage(IMessage message)
        {
            try
            {
                await _deliveryComplete.Task.ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                if (_log.IsErrorEnabled)
                    _log.Debug("Shutting down, deliveryComplete Faulted: {_topology.InputAddress}", exception);
            }
        }

        protected override async Task StopSupervisor(StopSupervisorContext context)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Stopping consumer: {0}", _context.InputAddress);

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
                        _log.WarnFormat("Stop canceled waiting for message consumers to complete: {0}", _context.InputAddress);
                }
            }

            try
            {
                _messageConsumer.Close();
                _messageConsumer.Dispose();
            }
            catch (OperationCanceledException)
            {
                if (_log.IsWarnEnabled)
                    _log.WarnFormat("Exception canceling the consumer: {0}", _context.InputAddress);
            }
        }
    }
}