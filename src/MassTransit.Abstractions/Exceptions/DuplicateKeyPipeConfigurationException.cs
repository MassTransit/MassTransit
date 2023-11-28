namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public class DuplicateKeyPipeConfigurationException :
        PipeConfigurationException
    {
        public DuplicateKeyPipeConfigurationException()
        {
        }

        public DuplicateKeyPipeConfigurationException(string message)
            : base(message)
        {
        }

        public DuplicateKeyPipeConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete("Formatter-based serialization is obsolete and should not be used.")]
#endif
        protected DuplicateKeyPipeConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
