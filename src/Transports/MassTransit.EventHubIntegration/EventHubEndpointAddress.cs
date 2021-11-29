namespace MassTransit
{
    using System;
    using System.Diagnostics;


    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
    public readonly struct EventHubEndpointAddress
    {
        public const string PathPrefix = "event-hub";

        public readonly string EventHubName;

        public readonly string Host;
        public readonly string Scheme;
        public readonly int? Port;

        public EventHubEndpointAddress(Uri hostAddress, Uri address)
        {
            Host = default;
            EventHubName = default;
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
        }

        public EventHubEndpointAddress(Uri hostAddress, string eventHubName)
        {
            ParseLeft(hostAddress, out Scheme, out Host, out Port);

            EventHubName = eventHubName;
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

            return builder.Uri;
        }

        Uri DebuggerDisplay => this;
    }
}
