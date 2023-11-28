namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class TransportUnavailableException :
        MassTransitException
    {
        public TransportUnavailableException()
        {
        }

        public TransportUnavailableException(string message)
            : base(message)
        {
        }

        public TransportUnavailableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected TransportUnavailableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
