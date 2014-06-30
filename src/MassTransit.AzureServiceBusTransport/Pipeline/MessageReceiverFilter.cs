// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AzureServiceBusTransport.Pipeline
{
    using System;
    using System.Threading.Tasks;
    using Logging;
    using MassTransit.Pipeline;
    using Microsoft.ServiceBus.Messaging;


    public class MessageReceiverFilter :
        IFilter<MessagingFactoryContext>
    {
        static readonly ILog _log = Logger.Get<MessageReceiverFilter>();
        readonly IPipe<ReceiveContext> _pipe;

        public MessageReceiverFilter(IPipe<ReceiveContext> pipe)
        {
            _pipe = pipe;
        }

        public async Task Send(MessagingFactoryContext context, IPipe<MessagingFactoryContext> next)
        {
            var receiveSettings = context.GetPayload<ReceiveSettings>();

            Uri inputAddress = context.GetQueueAddress(receiveSettings.QueueName);

            MessageReceiver messageReceiver = await context.Factory.CreateMessageReceiverAsync(receiveSettings.QueueName, ReceiveMode.PeekLock);
            messageReceiver.PrefetchCount = receiveSettings.PrefetchCount;

            using (var receiver = new Receiver(messageReceiver, inputAddress, _pipe, receiveSettings, context.CancellationToken))
            {
                ReceiverMetrics metrics = await receiver.CompleteTask;

                if (_log.IsDebugEnabled)
                {
                    _log.DebugFormat("Consumer {0}: {1} received, {2} concurrent", receiveSettings.QueueName, metrics.DeliveryCount,
                        metrics.ConcurrentDeliveryCount);
                }
            }

            await next.Send(context);
        }

        public bool Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this, (x, _) => _pipe.Inspect(inspector));
        }
    }
}