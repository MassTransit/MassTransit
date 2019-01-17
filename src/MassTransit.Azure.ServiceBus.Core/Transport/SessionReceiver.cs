// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using GreenPipes;
    using Logging;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Core;


    public class SessionReceiver :
        Receiver
    {
        static readonly ILog _log = Logger.Get<SessionReceiver>();

        readonly ClientContext _context;
        readonly IBrokeredMessageReceiver _messageReceiver;

        public SessionReceiver(ClientContext context, IBrokeredMessageReceiver messageReceiver)
            : base(context, messageReceiver)
        {
            _context = context;
            _messageReceiver = messageReceiver;
        }

        public override Task Start()
        {
            _context.OnSessionAsync(OnSession, ExceptionHandler);

            SetReady();

            return Task.CompletedTask;
        }

        async Task OnSession(IMessageSession messageSession, Message message, CancellationToken cancellationToken)
        {
            if (IsStopping)
            {
                await WaitAndAbandonMessage(messageSession, message).ConfigureAwait(false);
                return;
            }

            using (var delivery = Tracker.BeginDelivery())
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("Receiving {0}:{1}({2})", delivery.Id, message.MessageId, _context.EntityPath);

                await _messageReceiver.Handle(message, context => AddReceiveContextPayloads(context, messageSession)).ConfigureAwait(false);
            }
        }

        void AddReceiveContextPayloads(ReceiveContext receiveContext, IMessageSession messageSession)
        {
            var sessionContext = new BrokeredMessageSessionContext(messageSession);

            receiveContext.GetOrAddPayload<IReceiverClient>(() => messageSession);
            receiveContext.GetOrAddPayload(() => sessionContext);
            receiveContext.GetOrAddPayload(() => _context.GetPayload<NamespaceContext>());
        }
    }
}