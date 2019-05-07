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
namespace MassTransit.AzureServiceBusTransport.Transport
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Logging;
    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceBus.Messaging;
    using Transports.Metrics;
    using Util;


    public class Receiver :
        Supervisor,
        IReceiver
    {
        static readonly ILogger _logger = Logger.Get<Receiver>();
        readonly ClientContext _context;
        readonly TaskCompletionSource<bool> _deliveryComplete;
        readonly IBrokeredMessageReceiver _messageReceiver;
        readonly IDeliveryTracker _tracker;

        public Receiver(ClientContext context, IBrokeredMessageReceiver messageReceiver)
        {
            _context = context;
            _messageReceiver = messageReceiver;

            _tracker = new DeliveryTracker(HandleDeliveryComplete);
            _deliveryComplete = new TaskCompletionSource<bool>();
        }

        public Task DeliveryCompleted => _deliveryComplete.Task;
        bool IReceiver.IsStopping => IsStopping;

        public DeliveryMetrics GetDeliveryMetrics()
        {
            return _tracker.GetDeliveryMetrics();
        }

        public virtual Task Start()
        {
            _context.OnMessageAsync(OnMessage, ExceptionHandler);

            SetReady();

            return TaskUtil.Completed;
        }

        protected IDeliveryTracker DeliveryTracker => _tracker;

        protected void ExceptionHandler(object sender, ExceptionReceivedEventArgs args)
        {
            if (!(args.Exception is OperationCanceledException))
                _logger.LogError($"Exception received on receiver: {_context.InputAddress} during {args.Action}", args.Exception);

            if (_tracker.ActiveDeliveryCount == 0)
            {
                _logger.LogDebug("Receiver shutdown completed: {0}", _context.InputAddress);

                _deliveryComplete.TrySetResult(true);

                SetCompleted(TaskUtil.Faulted<bool>(args.Exception));
            }
        }

        void HandleDeliveryComplete()
        {
            if (IsStopping)
            {
                _logger.LogDebug("Receiver shutdown completed: {0}", _context.InputAddress);

                _deliveryComplete.TrySetResult(true);
            }
        }

        protected override async Task StopSupervisor(StopSupervisorContext context)
        {
            _logger.LogDebug("Stopping receiver: {0}", _context.InputAddress);

            SetCompleted(ActiveAndActualAgentsCompleted(context));

            await Completed.ConfigureAwait(false);
        }

        async Task ActiveAndActualAgentsCompleted(StopSupervisorContext context)
        {
            await Task.WhenAll(context.Agents.Select(x => Completed)).UntilCompletedOrCanceled(context.CancellationToken).ConfigureAwait(false);

            if (_tracker.ActiveDeliveryCount > 0)
                try
                {
                    await _deliveryComplete.Task.UntilCompletedOrCanceled(context.CancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Stop canceled waiting for message consumers to complete: {0}", _context.InputAddress);
                }
        }

        async Task OnMessage(BrokeredMessage message)
        {
            if (IsStopping)
            {
                await WaitAndAbandonMessage(message).ConfigureAwait(false);
                return;
            }

            using (var delivery = _tracker.BeginDelivery())
            {
                _logger.LogDebug("Receiving {0}:{1}({2})", delivery.Id, message.MessageId, _context.EntityPath);

                await _messageReceiver.Handle(message, AddReceiveContextPayloads).ConfigureAwait(false);
            }
        }

        void AddReceiveContextPayloads(ReceiveContext receiveContext)
        {
            receiveContext.GetOrAddPayload(() => _messageReceiver);
            receiveContext.GetOrAddPayload(() => _context.GetPayload<NamespaceContext>());
        }

        async Task WaitAndAbandonMessage(BrokeredMessage message)
        {
            try
            {
                await _deliveryComplete.Task.ConfigureAwait(false);

                await message.AbandonAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                _logger.LogDebug($"Stopping receiver, abandoned message faulted: {_context.InputAddress}", exception);
            }
        }
    }
}
