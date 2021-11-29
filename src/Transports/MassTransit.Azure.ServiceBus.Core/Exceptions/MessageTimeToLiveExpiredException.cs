namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class MessageTimeToLiveExpiredException :
        TransportException
    {
        public MessageTimeToLiveExpiredException()
        {
        }

        public MessageTimeToLiveExpiredException(Uri uri)
            : base(uri)
        {
        }

        public MessageTimeToLiveExpiredException(Uri uri, string message)
            : base(uri, message)
        {
        }

        public MessageTimeToLiveExpiredException(Uri uri, string message, Exception innerException)
            : base(uri, message, innerException)
        {
        }

        protected MessageTimeToLiveExpiredException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
