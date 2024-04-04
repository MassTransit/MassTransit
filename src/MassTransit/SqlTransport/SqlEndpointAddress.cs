namespace MassTransit
{
    #nullable enable
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Initializers;
    using Initializers.TypeConverters;
    using Internals;


    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
    public readonly struct SqlEndpointAddress
    {
        const string InstanceNameKey = "instance";
        const string AutoDeleteKey = "autodelete";
        const string TypeKey = "type";


        public enum AddressType
        {
            Queue = 0,
            Topic = 1
        }


        static readonly ITypeConverter<AddressType, string> _parseConverter = new EnumTypeConverter<AddressType>();

        public readonly string Scheme;
        public readonly string Host;
        public readonly string? InstanceName;
        public readonly int? Port;
        public readonly string VirtualHost;
        public readonly string? Area;
        public readonly string Name;

        public readonly TimeSpan? AutoDeleteOnIdle;
        public readonly AddressType Type;

        public SqlEndpointAddress(Uri hostAddress, Uri address, AddressType type = AddressType.Queue)
        {
            Port = default;

            AutoDeleteOnIdle = null;
            Type = type;

            ParseLeft(hostAddress, out Scheme, out Host, out InstanceName, out Port, out VirtualHost, out Area);

            var scheme = address.Scheme.ToLowerInvariant();
            switch (scheme)
            {
                case SqlHostAddress.DbScheme:

                    address.ParseHostPathAndEntityName(out _, out Name!);

                    if (string.IsNullOrWhiteSpace(Name))
                        throw new SqlEndpointAddressException(address, "Endpoint name must be specified");
                    break;

                case "queue":
                    Name = address.AbsolutePath;
                    break;

                case "topic":
                    Area = default;
                    Name = address.AbsolutePath;
                    Type = AddressType.Topic;
                    break;

                default:
                    throw new SqlEndpointAddressException(address, "Scheme is not supported");
            }

            foreach (var (key, value) in address.SplitQueryString())
            {
                switch (key)
                {
                    case AutoDeleteKey when int.TryParse(value, out var result):
                        AutoDeleteOnIdle = TimeSpan.FromSeconds(result);
                        break;

                    case TypeKey when value != null && _parseConverter.TryConvert(value, out var result):
                        Type = result;
                        break;
                }
            }

            if (Type != AddressType.Queue && AutoDeleteOnIdle.HasValue)
                AutoDeleteOnIdle = null;
        }

        public SqlEndpointAddress(Uri hostAddress, string name, TimeSpan? autoDeleteOnIdle = null, AddressType type = AddressType.Queue)
        {
            ParseLeft(hostAddress, out Scheme, out Host, out InstanceName, out Port, out VirtualHost, out Area);

            Name = name;

            AutoDeleteOnIdle = type == AddressType.Queue ? autoDeleteOnIdle : null;

            Type = type;

            if (type == AddressType.Topic)
                Area = default;
        }

        static void ParseLeft(Uri address, out string scheme, out string host, out string? instanceName, out int? port, out string virtualHost,
            out string? area)
        {
            var hostAddress = new SqlHostAddress(address);
            scheme = hostAddress.Scheme;
            host = hostAddress.Host;
            instanceName = hostAddress.InstanceName;
            port = address.IsDefaultPort ? null : address.Port;
            virtualHost = hostAddress.VirtualHost;
            area = hostAddress.Area;
        }

        public static implicit operator Uri(in SqlEndpointAddress address)
        {
            var path = address.VirtualHost == "/" ? "/" : Uri.EscapeDataString(address.VirtualHost);
            if (!string.IsNullOrWhiteSpace(address.Area) && address.Type == AddressType.Queue)
                path += "." + Uri.EscapeDataString(address.Area);
            if (path[path.Length - 1] != '/')
                path += '/';
            path += address.Name;

            var builder = new UriBuilder
            {
                Scheme = address.Scheme,
                Host = address.Host.Trim().Trim('(', ')'),
                Port = address.Port ?? -1,
                Path = path
            };

            builder.Query += string.Join("&", address.GetQueryStringOptions());

            return builder.Uri;
        }

        Uri DebuggerDisplay => this;

        IEnumerable<string> GetQueryStringOptions()
        {
            if (AutoDeleteOnIdle.HasValue && Type == AddressType.Queue)
                yield return $"{AutoDeleteKey}={AutoDeleteOnIdle.Value.TotalSeconds:F0}";

            if (Type != AddressType.Queue)
                yield return $"{TypeKey}=topic";

            if (!string.IsNullOrEmpty(InstanceName))
                yield return $"{InstanceNameKey}={InstanceName}";
        }
    }
}
