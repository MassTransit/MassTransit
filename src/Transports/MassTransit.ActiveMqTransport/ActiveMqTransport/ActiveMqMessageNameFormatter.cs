namespace MassTransit.ActiveMqTransport
{
    using System;
    using Transports;


    public class ActiveMqMessageNameFormatter :
        IMessageNameFormatter
    {
        readonly IMessageNameFormatter _formatter;

        public ActiveMqMessageNameFormatter()
        {
            _formatter = new DefaultMessageNameFormatter("::", "--", ".", "-");
        }

        public MessageName GetMessageName(Type type)
        {
            return _formatter.GetMessageName(type);
        }
    }
}
