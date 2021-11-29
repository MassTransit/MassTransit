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

        protected ProduceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
