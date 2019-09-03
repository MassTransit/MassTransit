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
namespace MassTransit.AmazonSqsTransport.Pipeline
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;
    using Amazon.SQS.Model;
    using Contexts;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Logging;
    using Topology;
    using Transports.Metrics;


    /// <summary>
    /// Receives messages from AmazonSQS, pushing them to the InboundPipe of the service endpoint.
    /// </summary>
    public sealed class AmazonSqsBasicConsumer :
        Supervisor,
        IBasicConsumer,
        DeliveryMetrics
    {
        readonly TaskCompletionSource<bool> _deliveryComplete;
        readonly Uri _inputAddress;
        readonly ILog _log = Logger.Get<AmazonSqsBasicConsumer>();
        readonly ClientContext _client;
        readonly ConcurrentDictionary<string, AmazonSqsReceiveContext> _pending;
        readonly ReceiveSettings _receiveSettings;
        readonly SqsReceiveEndpointContext _context;
        readonly IDeliveryTracker _tracker;

        /// <summary>
        /// The basic consumer receives messages pushed from the broker.
        /// </summary>
        /// <param name="client">The model context for the consumer</param>
        /// <param name="inputAddress">The input address for messages received by the consumer</param>
        /// <param name="context">The topology</param>
        public AmazonSqsBasicConsumer(ClientContext client, Uri inputAddress, SqsReceiveEndpointContext context)
        {
            _client = client;
            _inputAddress = inputAddress;
            _context = context;

            _tracker = new DeliveryTracker(HandleDeliveryComplete);

            _receiveSettings = client.GetPayload<ReceiveSettings>();

            _pending = new ConcurrentDictionary<string, AmazonSqsReceiveContext>();

            _deliveryComplete = new TaskCompletionSource<bool>();

            SetReady();
        }

        public static DateTime? FromUnixTime(string unixTime)
        {
            return long.TryParse(unixTime, out long seconds) ? Epoch.AddSeconds(seconds) : default(DateTime?);
        }

        static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public async Task HandleMessage(Message message)
        {
            if (IsStopping)
            {
                await WaitAndAbandonMessage().ConfigureAwait(false);
                return;
            }

            using (_tracker.BeginDelivery())
            {
                var redelivered = message.Attributes.TryGetValue("ApproximateReceiveCount", out var receiveCountStr)
                    && int.TryParse(receiveCountStr, out var receiveCount) && receiveCount > 1;

                var context = new AmazonSqsReceiveContext(_inputAddress, message, redelivered, _context);

                context.GetOrAddPayload(() => _receiveSettings);
                context.GetOrAddPayload(() => _client);
                context.GetOrAddPayload(() => _client.ConnectionContext);

                try
                {
                    if (!_pending.TryAdd(message.MessageId, context))
                        if (_log.IsErrorEnabled)
                            _log.ErrorFormat("Duplicate BasicDeliver: {0}", message.MessageId);

                    await _context.ReceiveObservers.PreReceive(context).ConfigureAwait(false);

                    await _context.ReceivePipe.Send(context).ConfigureAwait(false);

                    await context.ReceiveCompleted.ConfigureAwait(false);

                    await _client.DeleteMessage(_receiveSettings.EntityName, message.ReceiptHandle).ConfigureAwait(false);

                    await _context.ReceiveObservers.PostReceive(context).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await _context.ReceiveObservers.ReceiveFault(context, ex).ConfigureAwait(false);
                }
                finally
                {
                    _pending.TryRemove(message.MessageId, out _);

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

        async Task WaitAndAbandonMessage()
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
        }
    }
}