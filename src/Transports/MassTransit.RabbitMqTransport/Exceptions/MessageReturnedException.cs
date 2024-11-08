namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    /// <summary>
    /// Published when a RabbitMQ channel is closed and the message was not confirmed by the broker.
    /// </summary>
    [Serializable]
    public class MessageReturnedException :
        MassTransitException
    {
        public MessageReturnedException()
        {
        }

        public MessageReturnedException(string message)
            : base(message)
        {
        }

        public MessageReturnedException(string message, Exception innerException)
            : base(message, innerException)
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
