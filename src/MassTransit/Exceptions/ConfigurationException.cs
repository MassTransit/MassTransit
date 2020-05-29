namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;
    using Configurators;


    [Serializable]
    public class ConfigurationException :
        MassTransitException
    {
        public ConfigurationException()
        {
        }

        public ConfigurationException(ConfigurationResult result, string message)
            : base(message)
        {
            Result = result;
        }

        public ConfigurationException(ConfigurationResult result, string message, Exception innerException)
            : base(message, innerException)
        {
            Result = result;
        }

        public ConfigurationException(string message)
            : base(message)
        {
        }

        public ConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ConfigurationResult Result { get; protected set; }
    }
}
