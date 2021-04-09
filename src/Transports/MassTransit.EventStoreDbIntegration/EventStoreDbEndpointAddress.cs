using System;
using System.Diagnostics;
using System.Linq;

namespace MassTransit.EventStoreDbIntegration
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
    public readonly struct EventStoreDbEndpointAddress
    {
        public const string PathPrefix = "event-store-db";
        
        public readonly StreamCategory StreamCategory;

        public readonly string Host;
        public readonly string Scheme;
        public readonly int? Port;

        public EventStoreDbEndpointAddress(Uri hostAddress, Uri address)
        {
            Host = default;
            StreamCategory = default;
            Scheme = default;
            Port = default;

            if (string.Equals(address.Scheme, hostAddress.Scheme, StringComparison.InvariantCultureIgnoreCase))
            {
                ParseLeft(hostAddress, out Scheme, out Host, out Port);
                StreamCategory = StreamCategory.FromString(address.AbsolutePath.Split("/").Last());
            }
            else
                throw new ArgumentException($"The address scheme is not supported: {address.Scheme}", nameof(address));
        }

        public EventStoreDbEndpointAddress(Uri hostAddress, StreamCategory streamCategory)
        {
            ParseLeft(hostAddress, out Scheme, out Host, out Port);

            StreamCategory = streamCategory;
        }

        static void ParseLeft(Uri address, out string scheme, out string host, out int? port)
        {
            scheme = address.Scheme;
            host = address.Host;
            port = address.Port;
        }

        public static implicit operator Uri(in EventStoreDbEndpointAddress address)
        {
            var builder = new UriBuilder
            {
                Scheme = address.Scheme,
                Host = address.Host,
                Port = address.Port ?? 0,
                Path = $"{PathPrefix}/{address.StreamCategory}"
            };

            return builder.Uri;
        }

        Uri DebuggerDisplay => this;
    }
}
