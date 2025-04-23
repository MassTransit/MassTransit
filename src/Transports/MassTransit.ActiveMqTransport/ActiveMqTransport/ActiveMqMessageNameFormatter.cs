namespace MassTransit.ActiveMqTransport
{
    using System;
    using Transports;


    public class ActiveMqMessageNameFormatter :
        IMessageNameFormatter
    {
        readonly IMessageNameFormatter _formatter;

        public ActiveMqMessageNameFormatter()
            : this(true)
        {
        }

        public ActiveMqMessageNameFormatter(bool includeNamespace)
        {
            _formatter = new DefaultMessageNameFormatter("::", "--", ".", "-", includeNamespace);
        }

        public string GetMessageName(Type type)
        {
            return _formatter.GetMessageName(type);
        }
    }
}
