namespace MassTransit.Azure.ServiceBus.Core
{
    using System;
    using Transports;


    public class ServiceBusMessageNameFormatter :
        IMessageNameFormatter
    {
        readonly IMessageNameFormatter _formatter;

        public ServiceBusMessageNameFormatter(bool useTildeNamespaceSeparator)
        {
            _formatter = useTildeNamespaceSeparator
                ? new DefaultMessageNameFormatter("---", "--", "~", "-")
                : new DefaultMessageNameFormatter("---", "--", "/", "-");
        }

        public MessageName GetMessageName(Type type)
        {
            return _formatter.GetMessageName(type);
        }
    }
}
