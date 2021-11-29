namespace MassTransit
{
    using System;
    using System.Diagnostics;


    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
    public readonly struct KafkaTopicAddress
    {
        public const string PathPrefix = "kafka";

        public readonly string Topic;

        public readonly string Host;
        public readonly string Scheme;
        public readonly int? Port;

        public KafkaTopicAddress(Uri hostAddress, Uri address)
        {
            Host = default;
            Topic = default;
            Scheme = default;
            Port = default;

            var scheme = address.Scheme.ToLowerInvariant();
            switch (scheme)
            {
                case "topic":
                    ParseLeft(hostAddress, out Scheme, out Host, out Port);
                    Topic = address.AbsolutePath;
                    break;
                default:
                {
                    if (string.Equals(address.Scheme, hostAddress.Scheme, StringComparison.InvariantCultureIgnoreCase))
                    {
                        ParseLeft(hostAddress, out Scheme, out Host, out Port);
                        Topic = address.AbsolutePath.Replace($"{PathPrefix}/", "");
                    }
                    else
                        throw new ArgumentException($"The address scheme is not supported: {address.Scheme}", nameof(address));

                    break;
                }
            }
        }

        public KafkaTopicAddress(Uri hostAddress, string topic)
        {
            ParseLeft(hostAddress, out Scheme, out Host, out Port);

            Topic = topic;
        }

        static void ParseLeft(Uri address, out string scheme, out string host, out int? port)
        {
            scheme = address.Scheme;
            host = address.Host;
            port = address.Port;
        }

        public static implicit operator Uri(in KafkaTopicAddress address)
        {
            var builder = new UriBuilder
            {
                Scheme = address.Scheme,
                Host = address.Host,
                Port = address.Port ?? 0,
                Path = $"{PathPrefix}/{address.Topic}"
            };

            return builder.Uri;
        }

        Uri DebuggerDisplay => this;
    }
}
