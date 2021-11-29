namespace MassTransit
{
    using System;
    using System.Runtime.Serialization;


    [Serializable]
    public sealed class RabbitMqAddressException :
        ConfigurationException
    {
        const string DefaultHelpLink = "http://www.rabbitmq.com/specification.html";

        public RabbitMqAddressException()
        {
            HelpLink = DefaultHelpLink;
        }

        public RabbitMqAddressException(string message)
            : base(message)
        {
            HelpLink = DefaultHelpLink;
        }

        public RabbitMqAddressException(string message, Exception innerException)
            : base(message, innerException)
        {
            HelpLink = DefaultHelpLink;
        }

        public RabbitMqAddressException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
