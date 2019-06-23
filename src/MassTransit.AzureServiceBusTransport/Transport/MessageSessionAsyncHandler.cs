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
    using Context;
    using Contexts;
    using Microsoft.ServiceBus.Messaging;
    using Transports.Metrics;
    using Util;


    public class MessageSessionAsyncHandler :
        IMessageSessionAsyncHandler
    {
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

            using (_tracker.BeginDelivery())
            {
                await _messageReceiver.Handle(message, context =>
                {
                    context.GetOrAddPayload<MessageSessionContext>(() => new BrokeredMessageSessionContext(session));
                    context.GetOrAddPayload(() => _context);
                }).ConfigureAwait(false);
            }
        }

        public Task OnCloseSessionAsync(MessageSession session)
        {
            LogContext.Debug?.Log("Session closed: {SessionId} ({InputAddress})", session.SessionId, _context.InputAddress);

            return TaskUtil.Completed;
        }

        public Task OnSessionLostAsync(Exception exception)
        {
            LogContext.Debug?.Log("Session lost: {SessionId} ({InputAddress})", _session.SessionId, _context.InputAddress);

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
                LogContext.Error?.Log(exception, "Abandon message faulted during shutdown: {InputAddress}", _context.InputAddress);
            }
        }
    }
}
