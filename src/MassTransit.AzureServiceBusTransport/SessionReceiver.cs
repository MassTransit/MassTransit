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
    using GreenPipes;
    using Internals.Extensions;
    using Logging;
    using Microsoft.ServiceBus.Messaging;
    using Util;


    public class SessionReceiver :
        ReceiverMetrics,
        ISessionReceiver
    {
        static readonly ILog _log = Logger.Get<SessionReceiver>();
        readonly ClientContext _clientContext;
        readonly ClientSettings _clientSettings;
        readonly ITaskParticipant _participant;
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly IDeliveryTracker _tracker;
        bool _shuttingDown;

        public SessionReceiver(NamespaceContext context, ClientContext clientContext, IPipe<ReceiveContext> receivePipe,
            ClientSettings clientSettings, ITaskSupervisor supervisor)
        {
            _clientContext = clientContext;
            _receivePipe = receivePipe;
            _clientSettings = clientSettings;

            _tracker = new DeliveryTracker(HandleDeliveryComplete);

            _participant = supervisor.CreateParticipant($"{TypeMetadataCache<Receiver>.ShortName} - {clientContext.InputAddress}", Stop);

            var options = new SessionHandlerOptions
            {
                AutoComplete = false,
                AutoRenewTimeout = clientSettings.AutoRenewTimeout,
                MaxConcurrentSessions = clientSettings.MaxConcurrentCalls,
                MessageWaitTimeout = clientSettings.MessageWaitTimeout
            };

            options.ExceptionReceived += (sender, x) =>
            {
                if (!(x.Exception is OperationCanceledException))
                {
                    if (_log.IsErrorEnabled)
                        _log.Error($"Exception received on session receiver: {clientContext.InputAddress} during {x.Action}", x.Exception);
                }

                if (_tracker.ActiveDeliveryCount == 0)
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Session receiver shutdown completed: {0}", clientContext.InputAddress);

                    _participant.SetComplete();
                }
            };

            IMessageSessionAsyncHandlerFactory handlerFactory = new MessageSessionAsyncHandlerFactory(context, supervisor, this, _tracker);

            clientContext.RegisterSessionHandlerFactoryAsync(handlerFactory, options);

            _participant.SetReady();
        }

        bool ISessionReceiver.IsShuttingDown => _shuttingDown;
        string ISessionReceiver.QueuePath => _clientSettings.Path;
        Uri ISessionReceiver.InputAddress => _clientContext.InputAddress;
        IPipe<ReceiveContext> ISessionReceiver.ReceivePipe => _receivePipe;

        public long DeliveryCount => _tracker.DeliveryCount;

        public int ConcurrentDeliveryCount => _tracker.MaxConcurrentDeliveryCount;

        void HandleDeliveryComplete()
        {
            if (_shuttingDown)
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Receiver shutdown completed: {0}", _clientContext.InputAddress);

                _participant.SetComplete();
            }
        }

        async Task Stop()
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Shutting down receiver: {0}", _clientContext.InputAddress);

            _shuttingDown = true;

            if (_tracker.ActiveDeliveryCount > 0)
            {
                try
                {
                    using (var cancellation = new CancellationTokenSource(_clientSettings.LockDuration))
                    {
                        await _participant.ParticipantCompleted.WithCancellation(cancellation.Token).ConfigureAwait(false);
                    }
                }
                catch (TaskCanceledException)
                {
                    if (_log.IsWarnEnabled)
                        _log.WarnFormat("Timeout waiting for receiver to exit: {0}", _clientContext.InputAddress);
                }
            }

            if (_tracker.ActiveDeliveryCount == 0)
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Receiver shutdown completed: {0}", _clientContext.InputAddress);

                _participant.SetComplete();
            }
        }
    }
}