namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class PipeConfigurationException :
        MassTransitException
    {
        public PipeConfigurationException()
        {
        }

        public PipeConfigurationException(string message)
            : base(message)
        {
        }

        public PipeConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PipeConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
