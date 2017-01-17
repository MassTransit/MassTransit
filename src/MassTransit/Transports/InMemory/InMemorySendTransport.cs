// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Transports.InMemory
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Fabric;
    using GreenPipes;
    using Pipeline.Observables;
    using Util;


    /// <summary>
    /// Support in-memory message queue that is not durable, but supports parallel delivery of messages
    /// based on TPL usage.
    /// </summary>
    public class InMemorySendTransport :
        ISendTransport
    {
        readonly IInMemoryExchange _exchange;
        readonly SendObservable _sendObservable;

        public InMemorySendTransport(IInMemoryExchange exchange)
        {
            _exchange = exchange;

            _sendObservable = new SendObservable();
        }

        public string ExchangeName => _exchange.Name;

        async Task ISendTransport.Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancelSend)
        {
            var context = new InMemorySendContext<T>(message, cancelSend);

            try
            {
                await pipe.Send(context).ConfigureAwait(false);

                var messageId = context.MessageId ?? NewId.NextGuid();

                await _sendObservable.PreSend(context).ConfigureAwait(false);

                var transportMessage = new InMemoryTransportMessage(messageId, context.Body, context.ContentType.MediaType, TypeMetadataCache<T>.ShortName);

                await _exchange.Send(transportMessage).ConfigureAwait(false);

                context.LogSent();

                await _sendObservable.PostSend(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                context.LogFaulted(ex);

                await _sendObservable.SendFault(context, ex).ConfigureAwait(false);

                throw;
            }
        }

        async Task ISendTransport.Move(ReceiveContext context, IPipe<SendContext> pipe)
        {
            var messageId = GetMessageId(context);

            byte[] body;
            using (var bodyStream = context.GetBody())
            {
                body = await GetMessageBody(bodyStream).ConfigureAwait(false);
            }

            var messageType = "Unknown";
            InMemoryTransportMessage receivedMessage;
            if (context.TryGetPayload(out receivedMessage))
                messageType = receivedMessage.MessageType;

            var transportMessage = new InMemoryTransportMessage(messageId, body, context.ContentType.MediaType, messageType);

            await _exchange.Send(transportMessage).ConfigureAwait(false);
        }

        Task ISendTransport.Close()
        {
            // an in-memory send transport does not get disposed
            return TaskUtil.Completed;
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservable.Connect(observer);
        }

        async Task<byte[]> GetMessageBody(Stream body)
        {
            using (var ms = new MemoryStream())
            {
                await body.CopyToAsync(ms).ConfigureAwait(false);

                return ms.ToArray();
            }
        }

        static Guid GetMessageId(ReceiveContext context)
        {
            object messageIdValue;
            return context.TransportHeaders.TryGetHeader("MessageId", out messageIdValue)
                ? new Guid(messageIdValue.ToString())
                : NewId.NextGuid();
        }
    }
}