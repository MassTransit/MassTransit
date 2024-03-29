namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class MessageRetryLimitExceededException :
        TransportException
    {
        public MessageRetryLimitExceededException()
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected MessageRetryLimitExceededException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public MessageRetryLimitExceededException(Uri uri)
            : base(uri)
        {
        }

        public MessageRetryLimitExceededException(Uri uri, string message)
            : base(uri, message)
        {
        }

        public MessageRetryLimitExceededException(Uri uri, string message, Exception innerException)
            : base(uri, message, innerException)
        {
        }
    }
}
