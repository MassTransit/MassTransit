namespace MassTransit.RabbitMqTransport
{
    using System;
    using Transports;


    public class RabbitMqMessageNameFormatter :
        IMessageNameFormatter
    {
        readonly IMessageNameFormatter _formatter;

        public RabbitMqMessageNameFormatter()
        {
            _formatter = new DefaultMessageNameFormatter("::", "--", ":", "-");
        }

        public string GetMessageName(Type type)
        {
            return _formatter.GetMessageName(type);
        }
    }
}
