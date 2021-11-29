namespace MassTransit
{
    using System;
    using System.Diagnostics;
    using Internals;
    using Util;


    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
    public readonly struct InMemoryHostAddress
    {
        const string InMemorySchema = "loopback";

        public readonly string Scheme;
        public readonly string Host;
        public readonly string VirtualHost;

        public InMemoryHostAddress(Uri address)
        {
            Scheme = default;
            Host = default;
            VirtualHost = default;

            var scheme = address.Scheme.ToLowerInvariant();
            switch (scheme)
            {
                case InMemorySchema:
                    ParseLeft(address, out Scheme, out Host, out VirtualHost);
                    break;

                default:
                    throw new ArgumentException($"The address scheme is not supported: {address.Scheme}", nameof(address));
            }
        }

        static void ParseLeft(Uri address, out string scheme, out string host, out string virtualHost)
        {
            scheme = address.Scheme;
            host = address.Host;
            virtualHost = address.ParseHostPath();
        }

        public static implicit operator Uri(in InMemoryHostAddress address)
        {
            var builder = new UriBuilder
            {
                Scheme = address.Scheme,
                Host = address.Host,
                Path = address.VirtualHost == "/"
                    ? "/"
                    : $"/{Uri.EscapeDataString(address.VirtualHost)}"
            };

            return builder.Uri;
        }

        Uri DebuggerDisplay => this;
    }
}
