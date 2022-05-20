namespace MassTransit
{
    using System;
    using System.Diagnostics;
    using Internals;


    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
    public readonly struct ActiveMqHostAddress
    {
        public const string ActiveMqScheme = "activemq";

        public readonly string Scheme;
        public readonly string Host;
        public readonly int? Port;
        public readonly string VirtualHost;

        public ActiveMqHostAddress(Uri address)
        {
            Scheme = default;
            Host = default;
            Port = default;
            VirtualHost = default;

            var scheme = address.Scheme.ToLowerInvariant();
            switch (scheme)
            {
                case ActiveMqScheme:
                    ParseLeft(address, out Scheme, out Host, out Port, out VirtualHost);
                    break;

                default:
                    throw new ArgumentException($"The address scheme is not supported: {address.Scheme}", nameof(address));
            }
        }

        public ActiveMqHostAddress(string host, int? port, string virtualHost)
        {
            Scheme = ActiveMqScheme;
            Host = host;
            Port = port;
            VirtualHost = virtualHost;

            if (port <= 0)
                Port = 61616;
        }

        static void ParseLeft(Uri address, out string scheme, out string host, out int? port, out string virtualHost)
        {
            scheme = address.Scheme;
            host = address.Host;

            port = address.IsDefaultPort || address.Port <= 0
                ? 61616
                : address.Port;

            virtualHost = address.ParseHostPath();
        }

        public static implicit operator Uri(in ActiveMqHostAddress address)
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
                    ? "/"
                    : $"/{Uri.EscapeDataString(address.VirtualHost)}"
            };

            return builder.Uri;
        }

        Uri DebuggerDisplay => this;
    }
}
