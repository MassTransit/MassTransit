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

        protected EndpointNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
