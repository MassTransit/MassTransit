namespace MassTransit
{
    using MassTransit.Transports.Outbox.Entities;
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class OnRampSweeperSendException :
        MassTransitException
    {
        public OnRampSweeperSendException(OnRampMessage onRampMessage)
        {
            OnRampMessage = onRampMessage;
        }

        public OnRampSweeperSendException(OnRampMessage onRampMessage, string message)
            : base(message)
        {
            OnRampMessage = onRampMessage;
        }

        public OnRampSweeperSendException(OnRampMessage onRampMessage, string message, Exception innerException)
            :
            base(message, innerException)
        {
            OnRampMessage = onRampMessage;
        }

        protected OnRampSweeperSendException(SerializationInfo info, StreamingContext context)
            :
            base(info, context)
        {
        }

        public OnRampMessage OnRampMessage { get; }
    }
}
