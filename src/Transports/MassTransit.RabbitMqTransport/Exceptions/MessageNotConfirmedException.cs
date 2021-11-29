namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    /// <summary>
    /// Published when a RabbitMQ channel is closed and the message was not confirmed by the broker.
    /// </summary>
    [Serializable]
    public class MessageNotConfirmedException :
        TransportException
    {
        public MessageNotConfirmedException()
        {
        }

        public MessageNotConfirmedException(Uri uri, string reason)
            : base(uri, $"The message was not confirmed: {reason}")
        {
        }

        public MessageNotConfirmedException(Uri uri, Exception innerException)
            : base(uri, $"The message was not confirmed: {innerException.Message}", innerException)
        {
        }

        public MessageNotConfirmedException(Uri uri, string message, Exception innerException)
            : base(uri, message, innerException)
        {
        }

        protected MessageNotConfirmedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
