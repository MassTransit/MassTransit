namespace MassTransit
{
    using System;


    [Serializable]
    public class MessageException :
        MassTransitException
    {
        public MessageException(Type messageType, string message, Exception innerException)
            : base(message, innerException)
        {
            MessageType = messageType;
        }

        public MessageException(Type messageType, string message)
            : base(message)
        {
            MessageType = messageType;
        }

        public MessageException()
        {
        }

        public Type? MessageType { get; private set; }
    }
}
