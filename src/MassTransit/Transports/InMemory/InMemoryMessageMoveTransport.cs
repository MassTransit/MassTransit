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
namespace MassTransit.Transports.InMemory
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Context;
    using Fabric;


    public class InMemoryMessageMoveTransport
    {
        readonly IInMemoryExchange _exchange;

        protected InMemoryMessageMoveTransport(IInMemoryExchange exchange)
        {
            _exchange = exchange;
        }

        protected async Task Move(ReceiveContext context, Action<InMemoryTransportMessage, SendHeaders> preSend)
        {
            var messageId = GetMessageId(context);

            byte[] body = context.GetBody();

            var messageType = "Unknown";
            if (context.TryGetPayload(out InMemoryTransportMessage receivedMessage))
                messageType = receivedMessage.MessageType;

            var transportMessage = new InMemoryTransportMessage(messageId, body, context.ContentType.MediaType, messageType);

            SendHeaders headers = new DictionarySendHeaders(transportMessage.Headers);

            headers.SetHostHeaders();

            preSend(transportMessage, headers);

            transportMessage.Headers[MessageHeaders.Reason] = "dead-letter";

            await _exchange.Send(transportMessage).ConfigureAwait(false);
        }

        static Guid GetMessageId(ReceiveContext context)
        {
            return context.TransportHeaders.TryGetHeader("MessageId", out var messageIdValue)
                ? new Guid(messageIdValue.ToString())
                : NewId.NextGuid();
        }

        static async Task<byte[]> GetMessageBody(Stream body)
        {
            using (var ms = new MemoryStream())
            {
                await body.CopyToAsync(ms).ConfigureAwait(false);

                return ms.ToArray();
            }
        }
    }
}