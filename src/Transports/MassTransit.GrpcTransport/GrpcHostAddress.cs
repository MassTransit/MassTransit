namespace MassTransit
{
    using System;
    using System.Diagnostics;
    using Internals;


    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
    public readonly struct GrpcHostAddress
    {
        public readonly string Scheme;
        public readonly string Host;
        public readonly int Port;
        public readonly string VirtualHost;

        public GrpcHostAddress(Uri address)
        {
            Scheme = default;
            Host = default;
            Port = default;
            VirtualHost = default;

            var scheme = address.Scheme.ToLowerInvariant();
            switch (scheme)
            {
                case "http":
                case "https":
                    ParseLeft(address, out Scheme, out Host, out Port, out VirtualHost);
                    break;

                default:
                    throw new ArgumentException($"The address scheme is not supported: {address.Scheme}", nameof(address));
            }
        }

        static void ParseLeft(Uri address, out string scheme, out string host, out int port, out string virtualHost)
        {
            scheme = address.Scheme;
            host = address.Host;
            port = address.Port;
            virtualHost = address.ParseHostPath();
        }

        public static implicit operator Uri(in GrpcHostAddress address)
        {
            var builder = new UriBuilder
            {
                Scheme = address.Scheme,
                Host = address.Host,
                Port = address.Port,
                Path = address.VirtualHost == "/"
                    ? "/"
                    : $"/{Uri.EscapeDataString(address.VirtualHost)}"
            };

            return builder.Uri;
        }

        Uri DebuggerDisplay => this;
    }
}