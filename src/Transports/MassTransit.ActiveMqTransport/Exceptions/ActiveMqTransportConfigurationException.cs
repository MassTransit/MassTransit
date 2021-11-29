namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class ActiveMqTransportConfigurationException :
        ActiveMqTransportException
    {
        public ActiveMqTransportConfigurationException()
        {
        }

        public ActiveMqTransportConfigurationException(string message)
            : base(message)
        {
        }

        public ActiveMqTransportConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ActiveMqTransportConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
