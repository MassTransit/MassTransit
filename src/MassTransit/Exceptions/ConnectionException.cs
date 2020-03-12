namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class ConnectionException :
        MassTransitException
    {
        public ConnectionException()
        {
        }

        public ConnectionException(bool isTransient)
        {
            IsTransient = isTransient;
        }

        public ConnectionException(string message, bool isTransient = false)
            : base(message)
        {
            IsTransient = isTransient;
        }

        public ConnectionException(string message, Exception innerException, bool isTransient = true)
            : base(message, innerException)
        {
            IsTransient = isTransient;
        }

        protected ConnectionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public bool IsTransient { get; }
    }
}
