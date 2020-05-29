namespace MassTransit.ActiveMqTransport
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Initializers;
    using Initializers.TypeConverters;
    using Topology;
    using Util;


    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
    public readonly struct ActiveMqEndpointAddress
    {
        const string AutoDeleteKey = "autodelete";
        const string DurableKey = "durable";
        const string TemporaryKey = "temporary";
        const string TypeKey = "type";


        public enum AddressType
        {
            Queue = 0,
            Topic = 1
        }


        static readonly ITypeConverter<AddressType, string> _parseConverter = new EnumTypeConverter<AddressType>();

        public readonly string Scheme;
        public readonly string Host;
        public readonly int? Port;
        public readonly string VirtualHost;

        public readonly string Name;
        public readonly bool Durable;
        public readonly bool AutoDelete;
        public readonly AddressType Type;

        public ActiveMqEndpointAddress(Uri hostAddress, Uri address)
        {
            Scheme = default;
            Host = default;
            Port = default;
            VirtualHost = default;

            Durable = true;
            AutoDelete = false;
            Type = AddressType.Queue;

            var scheme = address.Scheme.ToLowerInvariant();
            if (scheme.EndsWith("s"))
                Port = 5671;

            switch (scheme)
            {
                case ActiveMqHostAddress.ActiveMqScheme:
                    Scheme = address.Scheme;
                    Host = address.Host;
                    Port = address.IsDefaultPort
                        ? 61616
                        : address.Port;

                    address.ParseHostPathAndEntityName(out VirtualHost, out Name);
                    break;

                case "queue":
                    ParseLeft(hostAddress, out Scheme, out Host, out Port, out VirtualHost);

                    Name = address.AbsolutePath;
                    break;

                case "topic":
                    ParseLeft(hostAddress, out Scheme, out Host, out Port, out VirtualHost);

                    Name = address.AbsolutePath;
                    Type = AddressType.Topic;
                    break;

                default:
                    throw new ArgumentException($"The address scheme is not supported: {address.Scheme}", nameof(address));
            }

            if (Name == "*")
                Name = NewId.Next().ToString("NS");

            ActiveMqEntityNameValidator.Validator.ThrowIfInvalidEntityName(Name);

            foreach (var (key, value) in address.SplitQueryString())
            {
                switch (key)
                {
                    case TemporaryKey when bool.TryParse(value, out var result):
                        AutoDelete = result;
                        Durable = !result;
                        break;

                    case DurableKey when bool.TryParse(value, out var result):
                        Durable = result;
                        break;

                    case AutoDeleteKey when bool.TryParse(value, out var result):
                        AutoDelete = result;
                        break;

                    case TypeKey when _parseConverter.TryConvert(value, out var result):
                        Type = result;
                        break;
                }
            }
        }

        public ActiveMqEndpointAddress(Uri hostAddress, string exchangeName, bool durable = true, bool autoDelete = false, AddressType type = AddressType.Queue)
        {
            ParseLeft(hostAddress, out Scheme, out Host, out Port, out VirtualHost);

            Name = exchangeName;

            Durable = durable;
            AutoDelete = autoDelete;
            Type = type;
        }

        ActiveMqEndpointAddress(string scheme, string host, int? port, string virtualHost, string name, bool durable, bool autoDelete,
            AddressType type = AddressType.Queue)
        {
            Scheme = scheme;
            Host = host;
            Port = port;
            VirtualHost = virtualHost;
            Name = name;
            Durable = durable;
            AutoDelete = autoDelete;
            Type = type;
        }

        public ActiveMqEndpointAddress GetDelayAddress()
        {
            var name = $"{Name}_delay";

            return new ActiveMqEndpointAddress(Scheme, Host, Port, VirtualHost, name, Durable, AutoDelete, Type);
        }

        static void ParseLeft(Uri address, out string scheme, out string host, out int? port, out string virtualHost)
        {
            var hostAddress = new ActiveMqHostAddress(address);
            scheme = hostAddress.Scheme;
            host = hostAddress.Host;
            port = hostAddress.Port;
            virtualHost = hostAddress.VirtualHost;
        }

        public static implicit operator Uri(in ActiveMqEndpointAddress address)
        {
            var builder = new UriBuilder
            {
                Scheme = address.Scheme,
                Host = address.Host,
                Port = address.Port.HasValue
                    ? address.Scheme.EndsWith("s", StringComparison.OrdinalIgnoreCase)
                        ? address.Port.Value == 5671 ? 0 : address.Port.Value
                        : address.Port.Value == 5672
                            ? 0
                            : address.Port.Value
                    : 0,
                Path = address.VirtualHost == "/"
                    ? $"/{address.Name}"
                    : $"/{Uri.EscapeDataString(address.VirtualHost)}/{address.Name}"
            };

            builder.Query += string.Join("&", address.GetQueryStringOptions());

            return builder.Uri;
        }

        Uri DebuggerDisplay => this;

        public Uri TopicAddress
        {
            get
            {
                var builder = new UriBuilder($"topic:{Name}");

                builder.Query += string.Join("&", GetQueryStringOptions());

                return builder.Uri;
            }
        }

        IEnumerable<string> GetQueryStringOptions()
        {
            if (!Durable && AutoDelete)
                yield return $"{TemporaryKey}=true";
            else if (!Durable)
                yield return $"{DurableKey}=false";
            else if (AutoDelete)
                yield return $"{AutoDeleteKey}=true";

            if (Type != AddressType.Queue)
                yield return $"{TypeKey}=topic";
        }
    }
}
