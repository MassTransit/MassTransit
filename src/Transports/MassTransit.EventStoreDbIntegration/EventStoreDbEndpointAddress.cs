using System;
using System.Diagnostics;

namespace MassTransit.EventStoreDbIntegration
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
    public readonly struct EventStoreDbEndpointAddress
    {
        public const string PathPrefix = "esdb";
        
        public readonly string StreamName;

        public readonly string Host;
        public readonly string Scheme;
        public readonly int? Port;

        public EventStoreDbEndpointAddress(Uri hostAddress, Uri address)
        {
            Host = default;
            StreamName = default;
            Scheme = default;
            Port = default;

            var scheme = address.Scheme.ToLowerInvariant();
            switch (scheme)
            {
                case "queue":
                case "stream":
                case "topic":
                    ParseLeft(hostAddress, out Scheme, out Host, out Port);
                    StreamName = address.AbsolutePath;
                    break;
                default:
                    {
                        if (string.Equals(address.Scheme, hostAddress.Scheme, StringComparison.InvariantCultureIgnoreCase))
                        {
                            ParseLeft(hostAddress, out Scheme, out Host, out Port);
                            StreamName = address.AbsolutePath.Replace($"{PathPrefix}/", "");
                        }
                        else
                            throw new ArgumentException($"The address scheme is not supported: {address.Scheme}", nameof(address));

                        break;
                    }
            }
        }

        public EventStoreDbEndpointAddress(Uri hostAddress, string streamName)
        {
            ParseLeft(hostAddress, out Scheme, out Host, out Port);
            StreamName = streamName;
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
                Path = $"{PathPrefix}/{address.StreamName}"
            };

            return builder.Uri;
        }

        Uri DebuggerDisplay => this;
    }
}
