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
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Contexts;
    using Logging;
    using MassTransit.Pipeline;
    using Microsoft.ServiceBus.Messaging;
    using Transports;
    using Util;


    /// <summary>
    /// Send messages to an azure transport using the message sender.
    /// 
    /// May be sensible to create a IBatchSendTransport that allows multiple
    /// messages to be sent as a single batch (perhaps using Tx support?)
    /// </summary>
    public class ServiceBusSendTransport :
        ISendTransport
    {
        static readonly ILog _log = Logger.Get<ServiceBusSendTransport>();

        readonly Connectable<ISendObserver> _observers;
        readonly MessageSender _sender;

        public ServiceBusSendTransport(MessageSender sender)
        {
            _observers = new Connectable<ISendObserver>();
            _sender = sender;
        }

        async Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancelSend)
        {
            var context = new ServiceBusSendContextImpl<T>(message, cancelSend);

            try
            {
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

                        await _observers.ForEach(x => x.PreSend(context));

                        await _sender.SendAsync(brokeredMessage);

                        _log.DebugFormat("SEND {0} ({1})", brokeredMessage.MessageId, _sender.Path);

                        await _observers.ForEach(x => x.PostSend(context));
                    }
                }
            }
            catch (Exception ex)
            {
                _observers.ForEach(x => x.SendFault(context, ex))
                    .Wait(cancelSend);

                throw;
            }
        }

        public async Task Move(ReceiveContext context)
        {
            BrokeredMessageContext messageContext;
            if (context.TryGetPayload(out messageContext))
            {
                using (Stream messageBodyStream = context.Body)
                {
                    using (var brokeredMessage = new BrokeredMessage(messageBodyStream))
                    {
                        brokeredMessage.ContentType = context.ContentType.MediaType;
                        brokeredMessage.ForcePersistence = messageContext.ForcePersistence;
                        brokeredMessage.TimeToLive = messageContext.TimeToLive;
                        brokeredMessage.CorrelationId = messageContext.CorrelationId;
                        brokeredMessage.MessageId = messageContext.MessageId;
                        brokeredMessage.Label = messageContext.Label;
                        brokeredMessage.PartitionKey = messageContext.PartitionKey;
                        brokeredMessage.ReplyTo = messageContext.ReplyTo;
                        brokeredMessage.ReplyToSessionId = messageContext.ReplyToSessionId;
                        brokeredMessage.SessionId = messageContext.SessionId;

                        await _sender.SendAsync(brokeredMessage);

                        _log.DebugFormat("MOVE {0} ({1} to {2})", brokeredMessage.MessageId, context.InputAddress, _sender.Path);
                    }
                }
            }
        }

        public ConnectHandle Connect(ISendObserver observer)
        {
            return _observers.Connect(observer);
        }
    }
}