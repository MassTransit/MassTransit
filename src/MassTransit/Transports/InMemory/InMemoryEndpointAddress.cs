namespace MassTransit.Transports.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Util;


    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
    public readonly struct InMemoryEndpointAddress
    {
        const string BindQueueKey = "bind";
        const string QueueNameKey = "queue";

        public readonly string Scheme;
        public readonly string Host;
        public readonly string VirtualHost;

        public readonly string Name;
        public readonly bool BindToQueue;
        public readonly string QueueName;

        public InMemoryEndpointAddress(Uri hostAddress, Uri address)
        {
            Scheme = default;
            Host = default;
            VirtualHost = default;

            BindToQueue = false;
            QueueName = default;

            var scheme = address.Scheme.ToLowerInvariant();
            switch (scheme)
            {
                case "loopback":
                    Scheme = address.Scheme;
                    Host = address.Host;

                    address.ParseHostPathAndEntityName(out VirtualHost, out Name);
                    break;

                case "queue":
                    ParseLeft(hostAddress, out Scheme, out Host, out VirtualHost);

                    Name = address.AbsolutePath;
                    BindToQueue = true;
                    break;

                case "exchange":
                case "topic":
                    ParseLeft(hostAddress, out Scheme, out Host, out VirtualHost);

                    Name = address.AbsolutePath;
                    break;

                default:
                    throw new ArgumentException($"The address scheme is not supported: {address.Scheme}", nameof(address));
            }

            if (Name == "*")
                Name = NewId.Next().ToString("NS");

            foreach ((string key, string value) in address.SplitQueryString())
            {
                switch (key)
                {
                    case BindQueueKey when bool.TryParse(value, out bool result):
                        BindToQueue = result;
                        break;

                    case QueueNameKey when !string.IsNullOrWhiteSpace(value):
                        QueueName = Uri.UnescapeDataString(value);
                        break;
                }
            }
        }

        public InMemoryEndpointAddress(Uri hostAddress, string exchangeName, bool bindToQueue = false, string queueName = default)
        {
            ParseLeft(hostAddress, out Scheme, out Host, out VirtualHost);

            Name = exchangeName;

            BindToQueue = bindToQueue;
            QueueName = queueName;
        }

        static void ParseLeft(Uri address, out string scheme, out string host, out string virtualHost)
        {
            var hostAddress = new InMemoryHostAddress(address);
            scheme = hostAddress.Scheme;
            host = hostAddress.Host;
            virtualHost = hostAddress.VirtualHost;
        }

        public static implicit operator Uri(in InMemoryEndpointAddress address)
        {
            var builder = new UriBuilder
            {
                Scheme = address.Scheme,
                Host = address.Host,
                Path = address.VirtualHost == "/"
                    ? $"/{address.Name}"
                    : $"/{Uri.EscapeDataString(address.VirtualHost)}/{address.Name}"
            };

            builder.Query += string.Join("&", address.GetQueryStringOptions());

            return builder.Uri;
        }

        Uri DebuggerDisplay => this;

        IEnumerable<string> GetQueryStringOptions()
        {
            if (BindToQueue)
                yield return $"{BindQueueKey}=true";
            if (!string.IsNullOrWhiteSpace(QueueName))
                yield return $"{QueueNameKey}={Uri.EscapeDataString(QueueName)}";
        }
    }
}
