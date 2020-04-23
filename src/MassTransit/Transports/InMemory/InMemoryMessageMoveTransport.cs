namespace MassTransit.Transports.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fabric;


    public class InMemoryMessageMoveTransport
    {
        readonly IInMemoryExchange _exchange;

        protected InMemoryMessageMoveTransport(IInMemoryExchange exchange)
        {
            _exchange = exchange;
        }

        protected async Task Move(ReceiveContext context, Action<InMemoryTransportMessage, IDictionary<string, object>> preSend)
        {
            var messageId = GetMessageId(context);

            byte[] body = context.GetBody();

            var messageType = "Unknown";
            if (context.TryGetPayload(out InMemoryTransportMessage receivedMessage))
                messageType = receivedMessage.MessageType;

            var transportMessage = new InMemoryTransportMessage(messageId, body, context.ContentType?.MediaType, messageType);

            transportMessage.Headers.SetHostHeaders();

            preSend(transportMessage, transportMessage.Headers);

            await _exchange.Send(transportMessage).ConfigureAwait(false);
        }

        static Guid GetMessageId(ReceiveContext context)
        {
            return context.TransportHeaders.TryGetHeader(MessageHeaders.MessageId, out var messageIdValue)
                ? new Guid(messageIdValue.ToString())
                : NewId.NextGuid();
        }
    }
}
