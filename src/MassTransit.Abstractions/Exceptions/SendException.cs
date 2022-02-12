namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class SendException :
        AbstractUriException
    {
        public SendException()
        {
        }

        public SendException(Type messageType, Uri uri)
            : base(uri)
        {
            MessageType = messageType;
        }

        public SendException(Type messageType, Uri uri, string message)
            : base(uri, message)
        {
            MessageType = messageType;
        }

        public SendException(Type messageType, Uri uri, string message, Exception innerException)
            : base(uri, message, innerException)
        {
            MessageType = messageType;
        }

        protected SendException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public Type? MessageType { get; protected set; }
    }
}
