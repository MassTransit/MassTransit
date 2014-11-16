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
namespace MassTransit.AzureServiceBusTransport
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Pipeline;
    using Microsoft.ServiceBus.Messaging;
    using Transports;


    /// <summary>
    /// Send messages to an azure transport using the message sender.
    /// 
    /// May be sensible to create a IBatchSendTransport that allows multiple
    /// messages to be sent as a single batch (perhaps using Tx support?)
    /// </summary>
    public class AzureServiceBusSendTransport :
        ISendTransport
    {
        readonly MessageSender _sender;

        public AzureServiceBusSendTransport(MessageSender sender)
        {
            _sender = sender;
        }

        async Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancelSend)
        {
            var context = new AzureServiceBusSendContextImpl<T>(message, cancelSend);

            await pipe.Send(context);

            using (Stream messageBodyStream = context.GetBodyStream())
            {
                using (var brokeredMessage = new BrokeredMessage(messageBodyStream))
                {
                    brokeredMessage.ContentType = context.ContentType.MediaType;
                    brokeredMessage.ForcePersistence = context.Durable;

                    if (context.TimeToLive.HasValue)
                        brokeredMessage.TimeToLive = context.TimeToLive.Value;

                    if (context.MessageId.HasValue)
                        brokeredMessage.MessageId = context.MessageId.Value.ToString("N");

                    if (context.CorrelationId.HasValue)
                        brokeredMessage.CorrelationId = context.CorrelationId.Value.ToString("N");

                    await _sender.SendAsync(brokeredMessage);
                }
            }
        }
    }
}