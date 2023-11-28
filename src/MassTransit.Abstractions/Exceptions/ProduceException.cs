namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class ProduceException :
        MassTransitException
    {
        public ProduceException()
        {
        }

        public ProduceException(string message)
            : base(message)
        {
        }

        public ProduceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected ProduceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
