namespace MassTransit.GrpcTransport.Integration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Fabric;


    public class GrpcMessageMoveTransport
    {
        readonly IGrpcExchange _exchange;

        protected GrpcMessageMoveTransport(IGrpcExchange exchange)
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
                    Deliver = new Deliver(receivedMessage.Message.Deliver) {Exchange = _exchange.Name}
                };

                var transportMessage = new GrpcTransportMessage(message, receivedMessage.Host);

                preSend(transportMessage, transportMessage.SendHeaders);

                await _exchange.Send(transportMessage, CancellationToken.None).ConfigureAwait(false);
            }
        }
    }
}
