#nullable enable
namespace MassTransit.SqlTransport
{
    using System;
    using System.Threading.Tasks;


    public class SqlQueueMoveTransport
    {
        readonly string _queueName;
        readonly SqlQueueType _queueType;

        protected SqlQueueMoveTransport(string queueName, SqlQueueType queueType)
        {
            _queueName = queueName;
            _queueType = queueType;
        }

        protected async Task Move(ReceiveContext context, Action<SqlTransportMessage, SendHeaders> preSend)
        {
            if (!context.TryGetPayload(out SqlMessageContext? messageContext))
                throw new ArgumentException("The ReceiveContext must contain a DbMessageContext", nameof(context));

            if (!context.TryGetPayload(out ClientContext? clientContext))
                throw new ArgumentException("The ReceiveContext must contain a ClientContext", nameof(context));

            if (!messageContext.LockId.HasValue)
                throw new ArgumentException("The LockId is not present", nameof(context));

            var message = messageContext.TransportMessage;

            var transportHeaders = SqlTransportMessage.DeserializeHeaders(message.TransportHeaders);

            preSend(message, transportHeaders);

            await clientContext.MoveMessage(messageContext.LockId.Value, messageContext.DeliveryMessageId, _queueName, _queueType, transportHeaders);
        }
    }
}
