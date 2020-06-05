namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Util;


    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
    public readonly struct KafkaTopicAddress
    {
        public const string PathPrefix = "kafka";
        const string PartitionKey = "partition";

        public readonly string Topic;
        public readonly int? Partition;

        public readonly string Host;
        public readonly string Scheme;
        public readonly int? Port;

        public KafkaTopicAddress(Uri hostAddress, Uri address)
        {
            Host = default;
            Topic = default;
            Partition = default;
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
                        Topic = address.AbsolutePath.Replace("kafka/", "");
                    }
                    else
                        throw new ArgumentException($"The address scheme is not supported: {address.Scheme}", nameof(address));

                    break;
                }
            }

            foreach (var (key, value) in address.SplitQueryString())
            {
                switch (key)
                {
                    case PartitionKey when int.TryParse(value, out var result):
                        Partition = result;
                        break;
                }
            }
        }

        public KafkaTopicAddress(Uri hostAddress, string topic, int? partition = default)
        {
            ParseLeft(hostAddress, out Scheme, out Host, out Port);

            Topic = topic;
            Partition = partition;
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

            builder.Query += string.Join("&", address.GetQueryStringOptions());

            return builder.Uri;
        }

        IEnumerable<string> GetQueryStringOptions()
        {
            if (Partition.HasValue && Partition.Value != Confluent.Kafka.Partition.Any)
                yield return $"{PartitionKey}={Partition}";
        }

        Uri DebuggerDisplay => this;
    }
}
