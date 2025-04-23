namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using Transports;


    public class ServiceBusMessageNameFormatter :
        IMessageNameFormatter
    {
        readonly IMessageNameFormatter _formatter;

        public ServiceBusMessageNameFormatter(string namespaceSeparator = null)
            : this(true, namespaceSeparator)
        {
        }

        public ServiceBusMessageNameFormatter(bool includeNamespace, string namespaceSeparator = null)
        {
            _formatter = string.IsNullOrWhiteSpace(namespaceSeparator)
                ? new DefaultMessageNameFormatter("---", "--", "/", "-", includeNamespace)
                : new DefaultMessageNameFormatter("---", "--", namespaceSeparator, "-", includeNamespace);
        }

        public string GetMessageName(Type type)
        {
            return _formatter.GetMessageName(type).Replace("[]", "__");
        }
    }
}
