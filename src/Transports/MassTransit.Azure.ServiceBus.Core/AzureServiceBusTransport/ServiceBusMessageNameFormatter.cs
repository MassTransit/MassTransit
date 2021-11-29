namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using Transports;


    public class ServiceBusMessageNameFormatter :
        IMessageNameFormatter
    {
        readonly IMessageNameFormatter _formatter;

        public ServiceBusMessageNameFormatter(string namespaceSeparator = default)
        {
            _formatter = string.IsNullOrWhiteSpace(namespaceSeparator)
                ? new DefaultMessageNameFormatter("---", "--", "/", "-")
                : new DefaultMessageNameFormatter("---", "--", namespaceSeparator, "-");
        }

        public MessageName GetMessageName(Type type)
        {
            return _formatter.GetMessageName(type);
        }
    }
}
