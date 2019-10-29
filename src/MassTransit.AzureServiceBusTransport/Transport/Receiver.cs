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
    using Context;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Microsoft.ServiceBus.Messaging;
    using Transports.Metrics;
    using Util;


    public class Receiver :
        Supervisor,
        IReceiver
    {
        readonly ClientContext _context;
        readonly TaskCompletionSource<bool> _deliveryComplete;
        readonly IBrokeredMessageReceiver _messageReceiver;
        readonly IDeliveryTracker _tracker;

        public Receiver(ClientContext context, IBrokeredMessageReceiver messageReceiver)
        {
            _context = context;
            _messageReceiver = messageReceiver;

            _tracker = new DeliveryTracker(HandleDeliveryComplete);
            _deliveryComplete = TaskUtil.GetTask<bool>();
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
                LogContext.Error?.Log(args.Exception, "Exception on Receiver {InputAddress} during {Action}", _context.InputAddress, args.Action);

            if (_tracker.ActiveDeliveryCount == 0)
            {
                LogContext.Debug?.Log("Receiver shutdown completed: {InputAddress}", _context.InputAddress);

                _deliveryComplete.TrySetResult(true);

                SetCompleted(TaskUtil.Faulted<bool>(args.Exception));
            }
        }

        void HandleDeliveryComplete()
        {
            if (IsStopping)
            {
                _deliveryComplete.TrySetResult(true);
            }
        }

        protected override async Task StopSupervisor(StopSupervisorContext context)
        {
            LogContext.Debug?.Log("Stopping receiver: {InputAddress}", _context.InputAddress);

            SetCompleted(ActiveAndActualAgentsCompleted(context));

            await Completed.ConfigureAwait(false);
        }

        async Task ActiveAndActualAgentsCompleted(StopSupervisorContext context)
        {
            await Task.WhenAll(context.Agents.Select(x => Completed)).OrCanceled(context.CancellationToken).ConfigureAwait(false);

            if (_tracker.ActiveDeliveryCount > 0)
                try
                {
                    await _deliveryComplete.Task.OrCanceled(context.CancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    LogContext.Warning?.Log("Stop canceled waiting for message consumers to complete: {InputAddress}", _context.InputAddress);
                }
        }

        async Task OnMessage(BrokeredMessage message)
        {
            if (IsStopping)
            {
                await WaitAndAbandonMessage(message).ConfigureAwait(false);
                return;
            }

            using (_tracker.BeginDelivery())
            {
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
                LogContext.Error?.Log(exception, "Abandon message faulted during shutdown: {InputAddress}", _context.InputAddress);
            }
        }
    }
}
