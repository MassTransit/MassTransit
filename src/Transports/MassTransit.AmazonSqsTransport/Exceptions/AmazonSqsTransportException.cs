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

        protected AmazonSqsTransportException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
