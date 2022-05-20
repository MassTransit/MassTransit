namespace MassTransit
{
    using System;
    using System.Diagnostics;
    using Internals;


    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
    public readonly struct RabbitMqHostAddress
    {
        const string HeartbeatKey = "heartbeat";
        const string PrefetchKey = "prefetch";
        public const string RabbitMqSchema = "rabbitmq";
        public const string RabbitMqSslSchema = "rabbitmqs";
        const string TimeToLiveKey = "ttl";

        public readonly string Scheme;
        public readonly string Host;
        public readonly int? Port;
        public readonly string VirtualHost;

        public readonly ushort? Heartbeat;
        public readonly ushort? Prefetch;
        public readonly int? TimeToLive;

        public RabbitMqHostAddress(Uri address)
        {
            Scheme = default;
            Host = default;
            Port = default;
            VirtualHost = default;

            var scheme = address.Scheme.ToLowerInvariant();
            switch (scheme)
            {
                case RabbitMqSslSchema:
                case "amqps":
                case RabbitMqSchema:
                case "amqp":
                    ParseLeft(address, out Scheme, out Host, out Port, out VirtualHost);
                    break;

                default:
                    throw new ArgumentException($"The address scheme is not supported: {address.Scheme}", nameof(address));
            }

            Heartbeat = default;
            Prefetch = default;
            TimeToLive = default;

            foreach (var (key, value) in address.SplitQueryString())
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

        public RabbitMqHostAddress(string host, int? port, string virtualHost)
        {
            Scheme = RabbitMqSchema;
            Host = host;
            Port = port;
            VirtualHost = virtualHost;

            if (port.HasValue)
            {
                if (port.Value <= 0)
                    Port = 5672;
                else if (port.Value == 5671)
                    Scheme = RabbitMqSslSchema;
            }

            Heartbeat = default;
            Prefetch = default;
            TimeToLive = default;
        }

        static void ParseLeft(Uri address, out string scheme, out string host, out int? port, out string virtualHost)
        {
            scheme = address.Scheme;
            host = address.Host;

            port = address.IsDefaultPort || address.Port <= 0
                ? scheme.EndsWith("s") ? 5671 : 5672
                : address.Port;

            virtualHost = address.ParseHostPath();
        }

        public static implicit operator Uri(in RabbitMqHostAddress address)
        {
            var builder = new UriBuilder
            {
                Scheme = address.Scheme,
                Host = address.Host,
                Port = address.Port.HasValue
                    ? address.Scheme.EndsWith("s", StringComparison.OrdinalIgnoreCase)
                        ? address.Port.Value == 5671 ? -1 : address.Port.Value
                        : address.Port.Value == 5672
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
