namespace MassTransit.ActiveMqTransport
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class ActiveMqConnectException :
        ActiveMqTransportException
    {
        public ActiveMqConnectException()
        {
        }

        public ActiveMqConnectException(string message)
            : base(message)
        {
        }

        public ActiveMqConnectException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ActiveMqConnectException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
