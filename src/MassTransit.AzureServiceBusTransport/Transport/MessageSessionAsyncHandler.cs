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
    using System.Threading.Tasks;
    using Contexts;
    using Logging;
    using MassTransit.Topology;
    using Microsoft.ServiceBus.Messaging;
    using Transports.Metrics;
    using Util;


    public class MessageSessionAsyncHandler :
        IMessageSessionAsyncHandler
    {
        static readonly ILog _log = Logger.Get<MessageSessionAsyncHandler>();
        readonly NamespaceContext _context;
        readonly ISessionReceiver _receiver;
        readonly MessageSession _session;
        readonly ITaskSupervisor _supervisor;
        readonly IDeliveryTracker _tracker;
        readonly IReceiveEndpointTopology _topology;

        public MessageSessionAsyncHandler(NamespaceContext context, ITaskSupervisor supervisor, ISessionReceiver receiver, MessageSession session,
            IDeliveryTracker tracker, IReceiveEndpointTopology topology)
        {
            _context = context;
            _supervisor = supervisor;
            _receiver = receiver;
            _session = session;
            _tracker = tracker;
            _topology = topology;
        }

        async Task IMessageSessionAsyncHandler.OnMessageAsync(MessageSession session, BrokeredMessage message)
        {
            if (_receiver.IsShuttingDown)
            {
                await WaitAndAbandonMessage(message).ConfigureAwait(false);
                return;
            }

            using (var delivery = _tracker.BeginDelivery())
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Receiving {0}:{1}({3}) - {2}", delivery.Id, message.MessageId, _receiver.QueuePath, session.SessionId);

                var context = new ServiceBusReceiveContext(_receiver.InputAddress, message, _context, _topology);

                context.GetOrAddPayload<MessageSessionContext>(() => new BrokeredMessageSessionContext(session));
                context.GetOrAddPayload(() => _context);

                try
                {
                    await _context.PreReceive(context).ConfigureAwait(false);

                    if (message.LockedUntilUtc <= DateTime.UtcNow)
                        throw new MessageLockExpiredException(_receiver.InputAddress, $"The message lock expired: {message.MessageId}");

                    if (message.ExpiresAtUtc < DateTime.UtcNow)
                        throw new MessageTimeToLiveExpiredException(_receiver.InputAddress, $"The message TTL expired: {message.MessageId}");

                    await _receiver.ReceivePipe.Send(context).ConfigureAwait(false);

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


        public Task OnCloseSessionAsync(MessageSession session)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Session Closed: {0} ({1})", session.SessionId, _receiver.InputAddress);

            return TaskUtil.Completed;
        }

        public Task OnSessionLostAsync(Exception exception)
        {
            if (_log.IsDebugEnabled)
                _log.Debug($"Session Closed: {_session.SessionId} ({_receiver.InputAddress})", exception);

            return TaskUtil.Completed;
        }

        async Task WaitAndAbandonMessage(BrokeredMessage message)
        {
            try
            {
                await _supervisor.Completed.ConfigureAwait(false);

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