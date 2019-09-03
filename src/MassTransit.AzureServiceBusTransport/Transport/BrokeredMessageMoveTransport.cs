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
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Context;
    using Contexts;
    using GreenPipes;
    using Microsoft.ServiceBus.Messaging;
    using Transports;


    public class BrokeredMessageMoveTransport
    {
        readonly IPipeContextSource<SendEndpointContext> _source;

        protected BrokeredMessageMoveTransport(IPipeContextSource<SendEndpointContext> source)
        {
            _source = source;
        }

        protected Task Move(ReceiveContext context, Action<BrokeredMessage, SendHeaders> preSend)
        {
            IPipe<SendEndpointContext> clientPipe = Pipe.ExecuteAsync<SendEndpointContext>(async clientContext =>
            {
                if (!context.TryGetPayload(out BrokeredMessageContext messageContext))
                    throw new ArgumentException("The ReceiveContext must contain a BrokeredMessageContext (from Azure Service Bus)", nameof(context));

                using (var messageBodyStream = context.GetBodyStream())
                using (var message = new BrokeredMessage(messageBodyStream)
                {
                    ContentType = context.ContentType.MediaType,
                    ForcePersistence = messageContext.ForcePersistence,
                    TimeToLive = messageContext.TimeToLive,
                    CorrelationId = messageContext.CorrelationId,
                    MessageId = messageContext.MessageId,
                    Label = messageContext.Label,
                    PartitionKey = messageContext.PartitionKey,
                    ReplyTo = messageContext.ReplyTo,
                    ReplyToSessionId = messageContext.ReplyToSessionId,
                    SessionId = messageContext.SessionId
                })
                {
                    SendHeaders headers = new DictionarySendHeaders(message.Properties);

                    foreach (KeyValuePair<string, object> property in messageContext.Properties)
                        headers.Set(property.Key, property.Value);

                    headers.SetHostHeaders();

                    preSend(message, headers);

                    await clientContext.Send(message).ConfigureAwait(false);

                    var reason = message.Properties.TryGetValue(MessageHeaders.Reason, out var reasonProperty) ? reasonProperty.ToString() : "";
                    if (reason == "fault")
                        reason = message.Properties.TryGetValue(MessageHeaders.FaultMessage, out var fault) ? $"Fault: {fault}" : "Fault";

                    context.LogMoved(clientContext.EntityPath, reason);
                }
            });

            return _source.Send(clientPipe, context.CancellationToken);
        }
    }
}