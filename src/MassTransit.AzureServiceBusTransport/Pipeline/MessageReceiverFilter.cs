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
namespace MassTransit.AzureServiceBusTransport.Pipeline
{
    using System;
    using System.Threading.Tasks;
    using Contexts;
    using Logging;
    using MassTransit.Pipeline;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Monitoring.Introspection;


    /// <summary>
    /// Creates a message receiver and receives messages from the input queue of the endpoint
    /// </summary>
    public class MessageReceiverFilter :
        IFilter<ConnectionContext>
    {
        static readonly ILog _log = Logger.Get<MessageReceiverFilter>();
        readonly INotifyReceiveObserver _receiveObserver;
        readonly IPipe<ReceiveContext> _receivePipe;

        public MessageReceiverFilter(IPipe<ReceiveContext> receivePipe, INotifyReceiveObserver receiveObserver)
        {
            _receivePipe = receivePipe;
            _receiveObserver = receiveObserver;
        }
        async void IProbeSite.Probe(ProbeContext context)
        {
        }

        async Task IFilter<ConnectionContext>.Send(ConnectionContext context, IPipe<ConnectionContext> next)
        {
            var receiveSettings = context.GetPayload<ReceiveSettings>();

            string queuePath = context.GetQueuePath(receiveSettings.QueueDescription);

            Uri inputAddress = context.GetQueueAddress(receiveSettings.QueueDescription);

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Creating message receiver for {0}", inputAddress);

            MessagingFactory messagingFactory = await context.MessagingFactory;
            MessageReceiver messageReceiver = await messagingFactory.CreateMessageReceiverAsync(queuePath, ReceiveMode.PeekLock);

            try
            {
                messageReceiver.PrefetchCount = receiveSettings.PrefetchCount;
                messageReceiver.RetryPolicy = RetryPolicy.Default;

                using (var receiver = new Receiver(messageReceiver, inputAddress, _receivePipe, receiveSettings, _receiveObserver, context.CancellationToken))
                {
                    ReceiverMetrics metrics = await receiver.CompleteTask;

                    if (_log.IsDebugEnabled)
                    {
                        _log.DebugFormat("Consumer {0}: {1} received, {2} concurrent", queuePath,
                            metrics.DeliveryCount,
                            metrics.ConcurrentDeliveryCount);
                    }
                }
            }
            catch
            {
                if (!messageReceiver.IsClosed)
                    messageReceiver.Close();

                throw;
            }

            if (!messageReceiver.IsClosed)
                await messageReceiver.CloseAsync();

            await next.Send(context);
        }
    }
}