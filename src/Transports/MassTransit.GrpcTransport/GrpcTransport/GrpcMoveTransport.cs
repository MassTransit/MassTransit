namespace MassTransit.GrpcTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Fabric;
    using Transports.Fabric;


    public class GrpcMoveTransport
    {
        readonly IMessageExchange<GrpcTransportMessage> _exchange;

        protected GrpcMoveTransport(IMessageExchange<GrpcTransportMessage> exchange)
        {
            _exchange = exchange;
        }

        protected async Task Move(ReceiveContext context, Action<GrpcTransportMessage, SendHeaders> preSend)
        {
            if (context.TryGetPayload(out GrpcTransportMessage receivedMessage))
            {
                var message = new TransportMessage
                {
                    MessageId = receivedMessage.Message.MessageId,
                    Deliver = new Deliver(receivedMessage.Message.Deliver) { Exchange = new ExchangeDestination { Name = _exchange.Name } }
                };

                var transportMessage = new GrpcTransportMessage(message, receivedMessage.Host);

                preSend(transportMessage, transportMessage.SendHeaders);

                var deliveryContext = new GrpcDeliveryContext(transportMessage, CancellationToken.None);

                await _exchange.Deliver(deliveryContext).ConfigureAwait(false);
            }
        }
    }
}
