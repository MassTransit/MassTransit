namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class AmazonSqsConnectionException :
        ConnectionException
    {
        public AmazonSqsConnectionException()
        {
        }

        public AmazonSqsConnectionException(string message)
            : base(message)
        {
        }

        public AmazonSqsConnectionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected AmazonSqsConnectionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
