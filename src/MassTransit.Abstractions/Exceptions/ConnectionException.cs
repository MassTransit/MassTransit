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

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected ConnectionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public bool IsTransient { get; }
    }
}
