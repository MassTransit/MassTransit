namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class PayloadFactoryException :
        PayloadException
    {
        public PayloadFactoryException()
        {
        }

        public PayloadFactoryException(string message)
            : base(message)
        {
        }

        public PayloadFactoryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PayloadFactoryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
