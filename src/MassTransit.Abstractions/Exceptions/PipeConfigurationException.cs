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

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected PipeConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
