namespace MassTransit.Azure.ServiceBus.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Initializers;
    using Initializers.TypeConverters;
    using Util;


    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
    public readonly struct ServiceBusEndpointAddress
    {
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
        public readonly string Scope;

        public readonly string Name;
        public readonly TimeSpan? AutoDelete;
        public readonly AddressType Type;

        public ServiceBusEndpointAddress(Uri hostAddress, Uri address)
        {
            Scheme = default;
            Host = default;
            Scope = default;

            AutoDelete = default;
            Type = AddressType.Queue;

            var scheme = address.Scheme.ToLowerInvariant();
            switch (scheme)
            {
                case "sb":
                    Scheme = address.Scheme;
                    Host = address.Host;

                    address.ParseHostPathAndEntityName(out Scope, out Name);
                    break;

                case "queue":
                    ParseLeft(hostAddress, out Scheme, out Host, out Scope);

                    Name = address.AbsolutePath;
                    break;

                case "topic":
                    ParseLeft(hostAddress, out Scheme, out Host, out Scope);

                    Name = address.AbsolutePath;
                    Type = AddressType.Topic;
                    break;

                default:
                    throw new ArgumentException($"The address scheme is not supported: {address.Scheme}", nameof(address));
            }

            foreach ((string key, string value) in address.SplitQueryString())
            {
                switch (key)
                {
                    case AutoDeleteKey when int.TryParse(value, out var result):
                        AutoDelete = TimeSpan.FromSeconds(result);
                        break;

                    case TypeKey when _parseConverter.TryConvert(value, out AddressType result):
                        Type = result;
                        break;
                }
            }
        }

        public ServiceBusEndpointAddress(Uri hostAddress, string name, TimeSpan? autoDelete = default, AddressType type = AddressType.Queue)
        {
            ParseLeft(hostAddress, out Scheme, out Host, out Scope);

            Name = name;

            AutoDelete = autoDelete;
            Type = type;
        }

        public string Path => Scope == "/" ? Name : $"{Scope}/{Name}";

        static void ParseLeft(Uri address, out string scheme, out string host, out string scope)
        {
            var hostAddress = new ServiceBusHostAddress(address);
            scheme = hostAddress.Scheme;
            host = hostAddress.Host;
            scope = hostAddress.Scope;
        }

        public static implicit operator Uri(in ServiceBusEndpointAddress address)
        {
            var builder = new UriBuilder
            {
                Scheme = address.Scheme,
                Host = address.Host,
                Path = address.Scope == "/" || address.Type == AddressType.Topic
                    ? $"/{address.Name}"
                    : $"/{address.Scope}/{address.Name}"
            };

            builder.Query += string.Join("&", address.GetQueryStringOptions());

            return builder.Uri;
        }

        Uri DebuggerDisplay => this;

        IEnumerable<string> GetQueryStringOptions()
        {
            if (AutoDelete.HasValue && AutoDelete.Value != Defaults.AutoDeleteOnIdle)
                yield return $"{AutoDeleteKey}={AutoDelete.Value.TotalSeconds:F0}";

            if (Type != AddressType.Queue)
                yield return $"{TypeKey}=Topic";
        }
    }
}
