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


    public interface ISessionReceiver
    {
        bool IsShuttingDown { get; }
        string QueuePath { get; }
        Uri InputAddress { get; }
        IReceiveObserver ReceiveObserver { get; }
        IPipe<ReceiveContext> ReceivePipe { get; }
        long IncrementDeliveryCount();
        void DeliveryComplete();
    }


    public class SessionReceiver :
        ReceiverMetrics,
        ISessionReceiver
    {
        static readonly ILog _log = Logger.Get<SessionReceiver>();

        readonly Uri _inputAddress;
        readonly ITaskParticipant _participant;
        readonly QueueClient _queueClient;
        readonly IReceiveObserver _receiveObserver;
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly ReceiveSettings _receiveSettings;
        int _currentPendingDeliveryCount;
        long _deliveryCount;
        int _maxPendingDeliveryCount;
        bool _shuttingDown;

        public SessionReceiver(ConnectionContext context, QueueClient queueClient, Uri inputAddress, IPipe<ReceiveContext> receivePipe, ReceiveSettings receiveSettings, IReceiveObserver receiveObserver, ITaskSupervisor supervisor)
        {
            _queueClient = queueClient;
            _inputAddress = inputAddress;
            _receivePipe = receivePipe;
            _receiveSettings = receiveSettings;
            _receiveObserver = receiveObserver;

            _participant = supervisor.CreateParticipant($"{TypeMetadataCache<Receiver>.ShortName} - {inputAddress}", Stop);

            var options = new SessionHandlerOptions
            {
                AutoComplete = false,
                AutoRenewTimeout = receiveSettings.AutoRenewTimeout,
                MaxConcurrentSessions = receiveSettings.MaxConcurrentCalls,
                MessageWaitTimeout = receiveSettings.MessageWaitTimeout
            };

            options.ExceptionReceived += (sender, x) =>
            {
                if (!(x.Exception is OperationCanceledException))
                {
                    if (_log.IsErrorEnabled)
                        _log.Error($"Exception received on session receiver: {_inputAddress} during {x.Action}", x.Exception);
                }

                if (_currentPendingDeliveryCount == 0)
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Session receiver shutdown completed: {0}", _inputAddress);

                    _participant.SetComplete();
                }
            };

            IMessageSessionAsyncHandlerFactory handlerFactory = new MessageSessionAsyncHandlerFactory(context, supervisor, this);
            queueClient.RegisterSessionHandlerFactoryAsync(handlerFactory, options);

            _participant.SetReady();
        }

        bool ISessionReceiver.IsShuttingDown => _shuttingDown;
        string ISessionReceiver.QueuePath => _receiveSettings.QueueDescription.Path;
        Uri ISessionReceiver.InputAddress => _inputAddress;
        IReceiveObserver ISessionReceiver.ReceiveObserver => _receiveObserver;
        IPipe<ReceiveContext> ISessionReceiver.ReceivePipe => _receivePipe;

        public long IncrementDeliveryCount()
        {
            var current = Interlocked.Increment(ref _currentPendingDeliveryCount);
            while (current > _maxPendingDeliveryCount)
                Interlocked.CompareExchange(ref _maxPendingDeliveryCount, current, _maxPendingDeliveryCount);

            return Interlocked.Increment(ref _deliveryCount);
        }

        public void DeliveryComplete()
        {
            var pendingCount = Interlocked.Decrement(ref _currentPendingDeliveryCount);
            if (pendingCount == 0 && _shuttingDown)
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Receiver shutdown completed: {0}", _inputAddress);

                _participant.SetComplete();
            }
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
                await _queueClient.CloseAsync().ConfigureAwait(false);
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
    }
}