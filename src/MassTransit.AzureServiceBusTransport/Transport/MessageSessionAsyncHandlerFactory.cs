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
    using Microsoft.ServiceBus.Messaging;
    using Transports.Metrics;


    public class MessageSessionAsyncHandlerFactory :
        IMessageSessionAsyncHandlerFactory
    {
        readonly ClientContext _context;
        readonly IBrokeredMessageReceiver _messageReceiver;
        readonly IReceiver _receiver;
        readonly IDeliveryTracker _tracker;

        public MessageSessionAsyncHandlerFactory(ClientContext context, IReceiver receiver, IDeliveryTracker tracker, IBrokeredMessageReceiver messageReceiver)
        {
            _context = context;
            _receiver = receiver;
            _tracker = tracker;
            _messageReceiver = messageReceiver;
        }

        public IMessageSessionAsyncHandler CreateInstance(MessageSession session, BrokeredMessage message)
        {
            return new MessageSessionAsyncHandler(_context, _receiver, session, _tracker, _messageReceiver);
        }

        public void DisposeInstance(IMessageSessionAsyncHandler handler)
        {
            var disposable = handler as IDisposable;
            disposable?.Dispose();
        }
    }
}