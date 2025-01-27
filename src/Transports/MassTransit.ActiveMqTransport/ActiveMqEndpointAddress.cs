namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using ActiveMqTransport.Topology;
    using Initializers;
    using Initializers.TypeConverters;
    using Internals;


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
            switch (scheme)
            {
                case ActiveMqHostAddress.AmqpScheme:
                case ActiveMqHostAddress.ActiveMqScheme:
                    ParseLeft(hostAddress, out Scheme, out Host, out Port, out VirtualHost);

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
                    ? address.Port.Value == 61616
                        ? -1
                        : address.Port.Value
                    : -1,
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
