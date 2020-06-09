namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Util;


    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
    public readonly struct EventHubEndpointAddress
    {
        public const string PathPrefix = "event-hub";
        const string PartitionKeyKey = "partitionKey";
        const string PartitionIdKey = "partitionId";

        public readonly string EventHubName;
        public readonly string PartitionKey;
        public readonly string PartitionId;

        public readonly string Host;
        public readonly string Scheme;
        public readonly int? Port;

        public EventHubEndpointAddress(Uri hostAddress, Uri address)
        {
            Host = default;
            EventHubName = default;
            PartitionKey = default;
            PartitionId = default;
            Scheme = default;
            Port = default;

            var scheme = address.Scheme.ToLowerInvariant();
            switch (scheme)
            {
                case "topic":
                    ParseLeft(hostAddress, out Scheme, out Host, out Port);
                    EventHubName = address.AbsolutePath;
                    break;
                default:
                {
                    if (string.Equals(address.Scheme, hostAddress.Scheme, StringComparison.InvariantCultureIgnoreCase))
                    {
                        ParseLeft(hostAddress, out Scheme, out Host, out Port);
                        EventHubName = address.AbsolutePath.Replace($"{PathPrefix}/", "");
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
                    case PartitionIdKey when !string.IsNullOrWhiteSpace(value):
                        PartitionId = value;
                        break;
                    case PartitionKeyKey when !string.IsNullOrWhiteSpace(value):
                        PartitionKey = value;
                        break;
                }
            }
        }

        public EventHubEndpointAddress(Uri hostAddress, string eventHubName, string partitionId = default, string partitionKey = default)
        {
            ParseLeft(hostAddress, out Scheme, out Host, out Port);

            EventHubName = eventHubName;
            PartitionKey = partitionKey;
            PartitionId = partitionId;
        }

        static void ParseLeft(Uri address, out string scheme, out string host, out int? port)
        {
            scheme = address.Scheme;
            host = address.Host;
            port = address.Port;
        }

        public static implicit operator Uri(in EventHubEndpointAddress address)
        {
            var builder = new UriBuilder
            {
                Scheme = address.Scheme,
                Host = address.Host,
                Port = address.Port ?? 0,
                Path = $"{PathPrefix}/{address.EventHubName}"
            };

            builder.Query += string.Join("&", address.GetQueryStringOptions());

            return builder.Uri;
        }

        IEnumerable<string> GetQueryStringOptions()
        {
            if (!string.IsNullOrWhiteSpace(PartitionId))
                yield return $"{PartitionIdKey}={PartitionId}";
            if (!string.IsNullOrWhiteSpace(PartitionKey))
                yield return $"{PartitionKeyKey}={PartitionKey}";
        }

        Uri DebuggerDisplay => this;
    }
}
