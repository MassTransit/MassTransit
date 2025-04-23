namespace MassTransit.RabbitMqTransport
{
    using System;
    using Transports;


    public class RabbitMqMessageNameFormatter :
        IMessageNameFormatter
    {
        readonly IMessageNameFormatter _formatter;

        public RabbitMqMessageNameFormatter()
            : this(true)
        {
        }

        public RabbitMqMessageNameFormatter(bool includeNamespace)
        {
            _formatter = new DefaultMessageNameFormatter("::", "--", ":", "-", includeNamespace);
        }

        public string GetMessageName(Type type)
        {
            return _formatter.GetMessageName(type);
        }
    }
}
