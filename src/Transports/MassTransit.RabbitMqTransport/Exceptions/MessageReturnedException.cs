namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    /// <summary>
    /// Published when a RabbitMQ channel is closed and the message was not confirmed by the broker.
    /// </summary>
    [Serializable]
    public class MessageReturnedException :
        TransportException
    {
        public MessageReturnedException()
        {
        }

        public MessageReturnedException(Uri uri, string message)
            : base(uri, message)
        {
        }

        public MessageReturnedException(Uri uri, string message, Exception innerException)
            : base(uri, message, innerException)
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected MessageReturnedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
