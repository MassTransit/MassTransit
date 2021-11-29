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

        protected DuplicateKeyPipeConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
