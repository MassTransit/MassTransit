namespace MassTransit.Transports.RabbitMq
{
    using System;
    using System.Runtime.Serialization;

    public class RabbitMqAddressException :
        Exception
    {
        public RabbitMqAddressException()
        {
            this.HelpLink = "http://www.rabbitmq.com/specification.html";
        }

        public RabbitMqAddressException(string message)
            : base(message)
        {
            this.HelpLink = "http://www.rabbitmq.com/specification.html";
        }

        public RabbitMqAddressException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.HelpLink = "http://www.rabbitmq.com/specification.html";
        }

        protected RabbitMqAddressException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.HelpLink = "http://www.rabbitmq.com/specification.html";
        }
    }
}