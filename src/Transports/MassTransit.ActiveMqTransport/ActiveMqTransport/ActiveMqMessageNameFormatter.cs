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

        public string GetMessageName(Type type)
        {
            return _formatter.GetMessageName(type);
        }
    }
}
