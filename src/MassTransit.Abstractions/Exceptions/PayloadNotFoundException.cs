namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class PayloadNotFoundException :
        PayloadException
    {
        public PayloadNotFoundException()
        {
        }

        public PayloadNotFoundException(string message)
            : base(message)
        {
        }

        public PayloadNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PayloadNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
