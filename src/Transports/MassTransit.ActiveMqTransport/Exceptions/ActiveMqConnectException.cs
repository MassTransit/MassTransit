namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Obsolete("Use ActiveMQConnectionException instead")]
    [Serializable]
    public class ActiveMqConnectException :
        ActiveMqConnectionException
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

    #if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
    #endif
        protected ActiveMqConnectException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
