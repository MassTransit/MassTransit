namespace MassTransit.InMemoryTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Fabric;
    using Transports;


    public class InMemoryMessageMoveTransport
    {
        readonly IInMemoryExchange _exchange;

        protected InMemoryMessageMoveTransport(IInMemoryExchange exchange)
        {
            _exchange = exchange;
        }

        protected async Task Move(ReceiveContext context, Action<InMemoryTransportMessage, IDictionary<string, object>> preSend)
        {
            var messageId = context.GetMessageId(NewId.NextGuid());

            var body = context.GetBody();

            var messageType = "Unknown";
            if (context.TryGetPayload(out InMemoryTransportMessage receivedMessage))
                messageType = receivedMessage.MessageType;

            var transportMessage = new InMemoryTransportMessage(messageId, body, context.ContentType?.MediaType, messageType);

            transportMessage.Headers.SetHostHeaders();

            preSend(transportMessage, transportMessage.Headers);

            await _exchange.Send(transportMessage, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
