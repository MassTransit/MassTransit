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
namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using Internals.Extensions;
    using Logging;
    using MassTransit.Pipeline;
    using Microsoft.ServiceBus.Messaging;
    using Util;


    public class Receiver :
        ReceiverMetrics
    {
        static readonly ILog _log = Logger.Get<Receiver>();

        readonly Uri _inputAddress;
        readonly MessageReceiver _messageReceiver;
        readonly ITaskParticipant _participant;
        readonly IReceiveObserver _receiveObserver;
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly ReceiveSettings _receiveSettings;
        int _currentPendingDeliveryCount;
        long _deliveryCount;
        int _maxPendingDeliveryCount;
        bool _shuttingDown;
        readonly ConnectionContext _context;

        public Receiver(ConnectionContext context, MessageReceiver messageReceiver, Uri inputAddress, IPipe<ReceiveContext> receivePipe, ReceiveSettings receiveSettings,
            IReceiveObserver receiveObserver, ITaskSupervisor supervisor)
        {
            _context = context;
            _messageReceiver = messageReceiver;
            _inputAddress = inputAddress;
            _receivePipe = receivePipe;
            _receiveSettings = receiveSettings;
            _receiveObserver = receiveObserver;

            _participant = supervisor.CreateParticipant($"{TypeMetadataCache<Receiver>.ShortName} - {inputAddress}", Stop);

            var options = new OnMessageOptions
            {
                AutoComplete = false,
                AutoRenewTimeout = receiveSettings.AutoRenewTimeout,
                MaxConcurrentCalls = receiveSettings.MaxConcurrentCalls
            };

            options.ExceptionReceived += (sender, x) =>
            {
                if (!(x.Exception is OperationCanceledException))
                {
                    if (_log.IsErrorEnabled)
                        _log.Error($"Exception received on receiver: {_inputAddress} during {x.Action}", x.Exception);
                }

                if (_currentPendingDeliveryCount == 0)
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Receiver shutdown completed: {0}", _inputAddress);

                    _participant.SetComplete();
                }
            };

            messageReceiver.OnMessageAsync(OnMessage, options);

            _participant.SetReady();
        }

        public long DeliveryCount => _deliveryCount;

        public int ConcurrentDeliveryCount => _maxPendingDeliveryCount;

        async Task Stop()
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Shutting down receiver: {0}", _inputAddress);

            _shuttingDown = true;

            if (_currentPendingDeliveryCount > 0)
            {
                try
                {
                    using (var cancellation = new CancellationTokenSource(_receiveSettings.QueueDescription.LockDuration))
                    {
                        await _participant.ParticipantCompleted.WithCancellation(cancellation.Token).ConfigureAwait(false);
                    }
                }
                catch (TaskCanceledException)
                {
                    if (_log.IsWarnEnabled)
                        _log.WarnFormat("Timeout waiting for receiver to exit: {0}", _inputAddress);
                }
            }

            try
            {
                await _messageReceiver.CloseAsync().ConfigureAwait(false);
            }
            finally
            {
                if (_currentPendingDeliveryCount == 0)
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Receiver shutdown completed: {0}", _inputAddress);

                    _participant.SetComplete();
                }
            }
        }

        async Task OnMessage(BrokeredMessage message)
        {
            if (_shuttingDown)
            {
                await WaitAndAbandonMessage(message).ConfigureAwait(false);
                return;
            }

            var current = Interlocked.Increment(ref _currentPendingDeliveryCount);
            while (current > _maxPendingDeliveryCount)
                Interlocked.CompareExchange(ref _maxPendingDeliveryCount, current, _maxPendingDeliveryCount);

            var deliveryCount = Interlocked.Increment(ref _deliveryCount);

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Receiving {0}:{1} - {2}", deliveryCount, message.MessageId, _receiveSettings.QueueDescription.Path);

            var context = new ServiceBusReceiveContext(_inputAddress, message, _receiveObserver);
            context.GetOrAddPayload(() => _context);

            try
            {
                await _receiveObserver.PreReceive(context).ConfigureAwait(false);

                await _receivePipe.Send(context).ConfigureAwait(false);

                await context.CompleteTask.ConfigureAwait(false);

                await message.CompleteAsync().ConfigureAwait(false);

                await _receiveObserver.PostReceive(context).ConfigureAwait(false);

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Receive completed: {0}", message.MessageId);
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                    _log.Error($"Received faulted: {message.MessageId}", ex);

                await message.AbandonAsync().ConfigureAwait(false);
                await _receiveObserver.ReceiveFault(context, ex).ConfigureAwait(false);
            }
            finally
            {
                var pendingCount = Interlocked.Decrement(ref _currentPendingDeliveryCount);
                if (pendingCount == 0 && _shuttingDown)
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Receiver shutdown completed: {0}", _inputAddress);

                    _participant.SetComplete();
                }
            }
        }

        async Task WaitAndAbandonMessage(BrokeredMessage message)
        {
            try
            {
                await _participant.ParticipantCompleted.ConfigureAwait(false);

                await message.AbandonAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                if (_log.IsErrorEnabled)
                    _log.Debug("Shutting down, abandoned message faulted: {_inputAddress}", exception);
            }
        }
    }
}