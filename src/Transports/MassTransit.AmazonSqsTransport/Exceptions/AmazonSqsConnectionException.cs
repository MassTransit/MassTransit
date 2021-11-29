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

        protected AmazonSqsConnectionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
