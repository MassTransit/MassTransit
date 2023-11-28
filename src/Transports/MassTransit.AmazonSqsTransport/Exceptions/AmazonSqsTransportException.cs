namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class AmazonSqsTransportException :
        MassTransitException
    {
        public AmazonSqsTransportException()
        {
        }

        public AmazonSqsTransportException(string message)
            : base(message)
        {
        }

        public AmazonSqsTransportException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected AmazonSqsTransportException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
