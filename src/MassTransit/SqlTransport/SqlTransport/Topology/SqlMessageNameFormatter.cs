#nullable enable
namespace MassTransit.SqlTransport.Topology
{
    using System;
    using Transports;


    public class SqlMessageNameFormatter :
        IMessageNameFormatter
    {
        readonly IMessageNameFormatter _formatter;

        public SqlMessageNameFormatter(string? namespaceSeparator = null)
        {
            _formatter = string.IsNullOrWhiteSpace(namespaceSeparator)
                ? new DefaultMessageNameFormatter("::", "--", ":", "-")
                : new DefaultMessageNameFormatter("::", "--", namespaceSeparator, "-");
        }

        public string GetMessageName(Type type)
        {
            return _formatter.GetMessageName(type);
        }
    }
}
