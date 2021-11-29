namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class ActiveMqConnectionException :
        ConnectionException
    {
        public ActiveMqConnectionException()
        {
        }

        public ActiveMqConnectionException(string message)
            : base(message)
        {
        }

        public ActiveMqConnectionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ActiveMqConnectionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
