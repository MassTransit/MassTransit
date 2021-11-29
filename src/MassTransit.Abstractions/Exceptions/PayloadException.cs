namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class PayloadException :
        MassTransitException
    {
        public PayloadException()
        {
        }

        public PayloadException(string message)
            : base(message)
        {
        }

        public PayloadException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PayloadException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
