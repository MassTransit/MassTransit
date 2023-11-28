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

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected SendException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public Type? MessageType { get; protected set; }
    }
}
