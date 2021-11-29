namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class ConsumerException :
        MassTransitException
    {
        public ConsumerException()
        {
        }

        public ConsumerException(string message)
            : base(message)
        {
        }

        public ConsumerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ConsumerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
