namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public sealed class AmazonSqsValidationException : ConfigurationException
    {
        public AmazonSqsValidationException()
        {
        }

        public AmazonSqsValidationException(string message)
            : base(message)
        {
        }

        public AmazonSqsValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected AmazonSqsValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
