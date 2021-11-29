namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class MessageDataException :
        MassTransitException
    {
        public MessageDataException()
        {
        }

        public MessageDataException(string message)
            : base(message)
        {
        }

        public MessageDataException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MessageDataException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
