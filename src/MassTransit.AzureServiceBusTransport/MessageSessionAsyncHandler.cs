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
    using System.Threading.Tasks;
    using Contexts;
    using Logging;
    using Microsoft.ServiceBus.Messaging;
    using Util;


    public class MessageSessionAsyncHandler :
        IMessageSessionAsyncHandler
    {
        static readonly ILog _log = Logger.Get<MessageSessionAsyncHandler>();
        readonly BrokeredMessage _message;
        readonly ISessionReceiver _receiver;
        readonly MessageSession _session;
        readonly ConnectionContext _context;
        readonly ITaskSupervisor _supervisor;

        public MessageSessionAsyncHandler(ConnectionContext context, ITaskSupervisor supervisor, ISessionReceiver receiver, MessageSession session, BrokeredMessage message)
        {
            _context = context;
            _supervisor = supervisor;
            _receiver = receiver;
            _session = session;
            _message = message;
        }

        async Task IMessageSessionAsyncHandler.OnMessageAsync(MessageSession session, BrokeredMessage message)
        {
            if (_receiver.IsShuttingDown)
            {
                await WaitAndAbandonMessage(message).ConfigureAwait(false);
                return;
            }

            var deliveryCount = _receiver.IncrementDeliveryCount();

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Receiving {0}:{1}({3}) - {2}", deliveryCount, message.MessageId, _receiver.QueuePath, session.SessionId);

            var context = new ServiceBusReceiveContext(_receiver.InputAddress, message, _receiver.ReceiveObserver);

            context.GetOrAddPayload<MessageSessionContext>(() => new BrokeredMessageSessionContext(session));
            context.GetOrAddPayload(() => _context);

            try
            {
                await _receiver.ReceiveObserver.PreReceive(context).ConfigureAwait(false);

                await _receiver.ReceivePipe.Send(context).ConfigureAwait(false);

                await context.CompleteTask.ConfigureAwait(false);

                await message.CompleteAsync().ConfigureAwait(false);

                await _receiver.ReceiveObserver.PostReceive(context).ConfigureAwait(false);

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Receive completed: {0}", message.MessageId);
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                    _log.Error($"Received faulted: {message.MessageId}", ex);

                await message.AbandonAsync().ConfigureAwait(false);
                await _receiver.ReceiveObserver.ReceiveFault(context, ex).ConfigureAwait(false);
            }
            finally
            {
                _receiver.DeliveryComplete();
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