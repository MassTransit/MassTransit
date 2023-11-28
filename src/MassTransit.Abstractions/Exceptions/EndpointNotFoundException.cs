namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class EndpointNotFoundException :
        MassTransitException
    {
        public EndpointNotFoundException()
        {
        }

        public EndpointNotFoundException(string message)
            : base(message)
        {
        }

        public EndpointNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected EndpointNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
