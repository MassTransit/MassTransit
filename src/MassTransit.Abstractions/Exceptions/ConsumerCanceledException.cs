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

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected ConsumerCanceledException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
