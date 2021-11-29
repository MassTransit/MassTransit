namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class ActiveMqTransportException :
        MassTransitException
    {
        public ActiveMqTransportException()
        {
        }

        public ActiveMqTransportException(string message)
            : base(message)
        {
        }

        public ActiveMqTransportException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ActiveMqTransportException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
