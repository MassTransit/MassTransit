namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class ConsumerCanceledException :
        MassTransitException
    {
        public ConsumerCanceledException()
        {
        }

        public ConsumerCanceledException(string message)
            : base(message)
        {
        }

        public ConsumerCanceledException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ConsumerCanceledException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
