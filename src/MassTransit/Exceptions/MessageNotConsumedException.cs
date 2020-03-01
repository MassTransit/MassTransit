namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class MessageNotConsumedException :
        TransportException
    {
        public MessageNotConsumedException()
        {
        }

        protected MessageNotConsumedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public MessageNotConsumedException(Uri uri)
            : base(uri)
        {
        }

        public MessageNotConsumedException(Uri uri, string message)
            : base(uri, message)
        {
        }

        public MessageNotConsumedException(Uri uri, string message, Exception innerException)
            : base(uri, message, innerException)
        {
        }
    }
}
