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
    using System.Threading.Tasks;
    using Microsoft.ServiceBus.Messaging;


    public class SessionReceiver :
        Receiver
    {
        readonly ClientContext _context;
        readonly IBrokeredMessageReceiver _messageReceiver;

        public SessionReceiver(ClientContext context, IBrokeredMessageReceiver messageReceiver)
            : base(context, messageReceiver)
        {
            _context = context;
            _messageReceiver = messageReceiver;
        }

        public override async Task Start()
        {
            IMessageSessionAsyncHandlerFactory handlerFactory = new MessageSessionAsyncHandlerFactory(_context, this, DeliveryTracker, _messageReceiver);

            await _context.RegisterSessionHandlerFactoryAsync(handlerFactory, ExceptionHandler).ConfigureAwait(false);

            SetReady();
        }
    }
}