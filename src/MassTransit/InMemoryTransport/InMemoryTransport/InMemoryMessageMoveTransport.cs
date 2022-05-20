namespace MassTransit.InMemoryTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Transports;
    using Transports.Fabric;


    public class InMemoryMessageMoveTransport
    {
        readonly IMessageExchange<InMemoryTransportMessage> _exchange;

        protected InMemoryMessageMoveTransport(IMessageExchange<InMemoryTransportMessage> exchange)
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

            var deliveryContext = new InMemoryDeliveryContext(transportMessage, CancellationToken.None);

            await _exchange.Deliver(deliveryContext).ConfigureAwait(false);
        }
    }
}
