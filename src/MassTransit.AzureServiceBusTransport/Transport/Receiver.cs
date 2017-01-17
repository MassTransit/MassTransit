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
namespace MassTransit.AzureServiceBusTransport.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using Internals.Extensions;
    using Logging;
    using MassTransit.Topology;
    using Microsoft.ServiceBus.Messaging;
    using Transports.Metrics;
    using Util;


    public class Receiver
    {
        static readonly ILog _log = Logger.Get<Receiver>();
        readonly ClientContext _clientContext;
        readonly ClientSettings _clientSettings;
        readonly NamespaceContext _context;
        readonly ITaskParticipant _participant;
        readonly IPipe<ReceiveContext> _receivePipe;
        readonly IDeliveryTracker _tracker;
        bool _shuttingDown;
        readonly IReceiveEndpointTopology _topology;

        public Receiver(NamespaceContext context, ClientContext clientContext, IPipe<ReceiveContext> receivePipe, ClientSettings clientSettings,
            ITaskSupervisor supervisor, IReceiveEndpointTopology topology)
        {
            _context = context;
            _clientContext = clientContext;
            _receivePipe = receivePipe;
            _clientSettings = clientSettings;
            _topology = topology;

            _tracker = new DeliveryTracker(DeliveryComplete);

            _participant = supervisor.CreateParticipant($"{TypeMetadataCache<Receiver>.ShortName} - {clientContext.InputAddress}", Stop);

            var options = new OnMessageOptions
            {
                AutoComplete = false,
                AutoRenewTimeout = clientSettings.AutoRenewTimeout,
                MaxConcurrentCalls = clientSettings.MaxConcurrentCalls
            };

            options.ExceptionReceived += (sender, x) =>
            {
                if (!(x.Exception is OperationCanceledException))
                {
                    if (_log.IsErrorEnabled)
                        _log.Error($"Exception received on receiver: {clientContext.InputAddress} during {x.Action}", x.Exception);
                }

                if (_tracker.ActiveDeliveryCount == 0)
                {
                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Receiver shutdown completed: {0}", clientContext.InputAddress);

                    _participant.SetComplete();
                }
            };

            clientContext.OnMessageAsync(OnMessage, options);

            _participant.SetReady();
        }

        public DeliveryMetrics GetDeliveryMetrics()
        {
            return _tracker.GetDeliveryMetrics();
        }

        void DeliveryComplete()
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

        async Task OnMessage(BrokeredMessage message)
        {
            if (_shuttingDown)
            {
                await WaitAndAbandonMessage(message).ConfigureAwait(false);
                return;
            }

            using (var delivery = _tracker.BeginDelivery())
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Receiving {0}:{1} - {2}", delivery.Id, message.MessageId, _clientSettings.Path);

                var context = new ServiceBusReceiveContext(_clientContext.InputAddress, message, _context, _topology);
                context.GetOrAddPayload(() => _context);

                try
                {
                    await _context.PreReceive(context).ConfigureAwait(false);

                    if (message.LockedUntilUtc <= DateTime.UtcNow)
                        throw new MessageLockExpiredException(_clientContext.InputAddress, $"The message lock expired: {message.MessageId}");

                    if (message.ExpiresAtUtc < DateTime.UtcNow)
                        throw new MessageTimeToLiveExpiredException(_clientContext.InputAddress, $"The message TTL expired: {message.MessageId}");

                    await _receivePipe.Send(context).ConfigureAwait(false);

                    await context.CompleteTask.ConfigureAwait(false);

                    await message.CompleteAsync().ConfigureAwait(false);

                    await _context.PostReceive(context).ConfigureAwait(false);

                    if (_log.IsDebugEnabled)
                        _log.DebugFormat("Receive completed: {0}", message.MessageId);
                }
                catch (Exception ex)
                {
                    if (_log.IsErrorEnabled)
                        _log.Error($"Received faulted: {message.MessageId}", ex);

                    await AbandonMessage(message).ConfigureAwait(false);

                    await _context.ReceiveFault(context, ex).ConfigureAwait(false);
                }
                finally
                {
                    context.Dispose();
                }
            }
        }

        static async Task AbandonMessage(BrokeredMessage message)
        {
            try
            {
                await message.AbandonAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                if (_log.IsWarnEnabled)
                    _log.Warn($"Abandon message faulted: {message.MessageId}", exception);
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
                    _log.Debug("Shutting down, abandoned message faulted: {_clientContext.InputAddress}", exception);
            }
        }
    }
}