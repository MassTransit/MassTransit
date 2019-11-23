namespace MassTransit.ActiveMqTransport
{
    using System;
    using System.Diagnostics;
    using System.Linq;


    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
    public readonly struct ActiveMqHostAddress
    {
        public const string ActiveMqScheme = "activemq";
        const string HeartbeatKey = "heartbeat";
        const string PrefetchKey = "prefetch";
        const string TimeToLiveKey = "ttl";

        public readonly string Scheme;
        public readonly string Host;
        public readonly int? Port;
        public readonly string VirtualHost;

        public readonly ushort? Heartbeat;
        public readonly ushort? Prefetch;
        public readonly int? TimeToLive;

        public ActiveMqHostAddress(string host, int? port, string virtualHost)
        {
            Scheme = ActiveMqScheme;
            Host = host;
            Port = port;
            VirtualHost = virtualHost;

            if (port.HasValue && port.Value == 0)
            {
                Port = 61616;
            }

            Heartbeat = default;
            Prefetch = default;
            TimeToLive = default;
        }

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

            Heartbeat = default;
            Prefetch = default;
            TimeToLive = default;

            string query = address.Query?.TrimStart('?');
            if (!string.IsNullOrWhiteSpace(query))
            {
                var parameters = query.Split('&').Select(x => x.Split('=')).Select(x => (x.First().ToLowerInvariant(), x.Skip(1).FirstOrDefault()));

                foreach ((string key, string value) in parameters)
                {
                    switch (key)
                    {
                        case HeartbeatKey when ushort.TryParse(value, out var result):
                            Heartbeat = result;
                            break;

                        case PrefetchKey when ushort.TryParse(value, out var result):
                            Prefetch = result;
                            break;

                        case TimeToLiveKey when int.TryParse(value, out var result):
                            TimeToLive = result;
                            break;
                    }
                }
            }
        }

        static void ParseLeft(Uri address, out string scheme, out string host, out int? port, out string virtualHost)
        {
            scheme = address.Scheme;
            host = address.Host;

            port = address.IsDefaultPort
                ? 61616
                : address.Port;

            virtualHost = ParseVirtualHost(address.AbsolutePath);
        }

        static string ParseVirtualHost(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return "/";

            if (path.Length == 1 && path[0] == '/')
                return path;

            int split = path.LastIndexOf('/');
            if (split > 0)
                return Uri.UnescapeDataString(path.Substring(1, split - 1));

            return Uri.UnescapeDataString(path.Substring(1));
        }

        public static implicit operator Uri(in ActiveMqHostAddress address)
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
                    ? "/"
                    : $"/{Uri.EscapeDataString(address.VirtualHost)}"
            };

            return builder.Uri;
        }

        Uri DebuggerDisplay => this;
    }
}
