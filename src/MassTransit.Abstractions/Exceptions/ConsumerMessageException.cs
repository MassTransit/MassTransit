namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class ConsumerMessageException :
        ConsumerException
    {
        public ConsumerMessageException()
        {
        }

        public ConsumerMessageException(string message)
            : base(message)
        {
        }

        public ConsumerMessageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected ConsumerMessageException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
