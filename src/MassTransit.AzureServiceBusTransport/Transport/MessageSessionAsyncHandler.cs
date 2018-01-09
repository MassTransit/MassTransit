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
    using Microsoft.ServiceBus.Messaging;
    using Transports.Metrics;
    using Util;


    public class MessageSessionAsyncHandler :
        IMessageSessionAsyncHandler
    {
        static readonly ILog _log = Logger.Get<MessageSessionAsyncHandler>();
        readonly ClientContext _context;
        readonly IReceiver _receiver;
        readonly MessageSession _session;
        readonly IDeliveryTracker _tracker;
        readonly IBrokeredMessageReceiver _messageReceiver;

        public MessageSessionAsyncHandler(ClientContext context, IReceiver receiver, MessageSession session, IDeliveryTracker tracker, IBrokeredMessageReceiver messageReceiver)
        {
            _context = context;
            _receiver = receiver;
            _session = session;
            _tracker = tracker;
            _messageReceiver = messageReceiver;
        }

        async Task IMessageSessionAsyncHandler.OnMessageAsync(MessageSession session, BrokeredMessage message)
        {
            if (_receiver.IsStopping)
            {
                await WaitAndAbandonMessage(message).ConfigureAwait(false);
                return;
            }

            using (var delivery = _tracker.BeginDelivery())
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Receiving {0}:{1}({3}) - {2}", delivery.Id, message.MessageId, _context.EntityPath, session.SessionId);

                await _messageReceiver.Handle(message, context =>
                {
                    context.GetOrAddPayload<MessageSessionContext>(() => new BrokeredMessageSessionContext(session));
                    context.GetOrAddPayload(() => _context);
                }).ConfigureAwait(false);
            }
        }

        public Task OnCloseSessionAsync(MessageSession session)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Session Closed: {0} ({1})", session.SessionId, _context.InputAddress);

            return TaskUtil.Completed;
        }

        public Task OnSessionLostAsync(Exception exception)
        {
            if (_log.IsDebugEnabled)
                _log.Debug($"Session Closed: {_session.SessionId} ({_context.InputAddress})", exception);

            return TaskUtil.Completed;
        }

        async Task WaitAndAbandonMessage(BrokeredMessage message)
        {
            try
            {
                await _receiver.DeliveryCompleted.ConfigureAwait(false);

                await message.AbandonAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                if (_log.IsErrorEnabled)
                    _log.Debug("Stopping async handler, abandoned message faulted: {_inputAddress}", exception);
            }
        }
    }
}