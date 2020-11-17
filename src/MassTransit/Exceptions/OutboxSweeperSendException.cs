namespace MassTransit
{
    using MassTransit.Transports.Outbox.Entities;
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class OutboxSweeperSendException :
        MassTransitException
    {
        public OutboxSweeperSendException(OutboxMessage outboxMessage)
        {
            OutboxMessage = outboxMessage;
        }

        public OutboxSweeperSendException(OutboxMessage outboxMessage, string message)
            : base(message)
        {
            OutboxMessage = outboxMessage;
        }

        public OutboxSweeperSendException(OutboxMessage outboxMessage, string message, Exception innerException)
            :
            base(message, innerException)
        {
            OutboxMessage = outboxMessage;
        }

        protected OutboxSweeperSendException(SerializationInfo info, StreamingContext context)
            :
            base(info, context)
        {
        }

        public OutboxMessage OutboxMessage { get; }
    }
}
