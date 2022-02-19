namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Internals;
    using Transports.Fabric;


    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
    public readonly struct GrpcEndpointAddress
    {
        const string BindQueueKey = "bind";
        const string QueueNameKey = "queue";
        const string ExchangeTypeKey = "type";
        const string InstanceIdKey = "instance";

        public readonly string Scheme;
        public readonly string Host;
        public readonly int Port;
        public readonly string VirtualHost;

        public readonly string Name;
        public readonly ExchangeType ExchangeType;
        public readonly bool BindToQueue;
        public readonly string QueueName;
        public readonly string InstanceId;

        public GrpcEndpointAddress(Uri hostAddress, Uri address)
        {
            Scheme = default;
            Host = default;
            Port = default;
            VirtualHost = default;

            ExchangeType = ExchangeType.FanOut;

            BindToQueue = false;
            QueueName = default;
            InstanceId = default;

            var scheme = address.Scheme.ToLowerInvariant();
            switch (scheme)
            {
                case "http":
                case "https":
                    Scheme = scheme;
                    Host = address.Host;
                    Port = address.Port;

                    address.ParseHostPathAndEntityName(out VirtualHost, out Name);
                    break;

                case "queue":
                    ParseLeft(hostAddress, out Scheme, out Host, out Port, out VirtualHost);

                    Name = address.AbsolutePath;
                    BindToQueue = true;
                    break;

                case "exchange":
                case "topic":
                    ParseLeft(hostAddress, out Scheme, out Host, out Port, out VirtualHost);

                    Name = address.AbsolutePath;
                    break;

                default:
                    throw new ArgumentException($"The address scheme is not supported: {address.Scheme}", nameof(address));
            }

            if (Name == "*")
                Name = NewId.Next().ToString("NS");

            foreach (var (key, value) in address.SplitQueryString())
            {
                switch (key)
                {
                    case ExchangeTypeKey when Enum.TryParse<ExchangeType>(value, out var result):
                        ExchangeType = result;
                        break;

                    case BindQueueKey when bool.TryParse(value, out var result):
                        BindToQueue = result;
                        break;

                    case QueueNameKey when !string.IsNullOrWhiteSpace(value):
                        QueueName = Uri.UnescapeDataString(value);
                        break;

                    case InstanceIdKey when !string.IsNullOrWhiteSpace(value):
                        InstanceId = value;
                        break;
                }
            }
        }

        public GrpcEndpointAddress(Uri hostAddress, string exchangeName, bool bindToQueue = false, string queueName = default,
            ExchangeType exchangeType = ExchangeType.FanOut, string instanceId = default)
        {
            ParseLeft(hostAddress, out Scheme, out Host, out Port, out VirtualHost);

            Name = exchangeName;
            ExchangeType = exchangeType;

            BindToQueue = bindToQueue;
            QueueName = queueName;
            InstanceId = instanceId;
        }

        static void ParseLeft(Uri address, out string scheme, out string host, out int port, out string virtualHost)
        {
            var hostAddress = new GrpcHostAddress(address);
            scheme = hostAddress.Scheme;
            host = hostAddress.Host;
            port = hostAddress.Port;
            virtualHost = hostAddress.VirtualHost;
        }

        public static implicit operator Uri(in GrpcEndpointAddress address)
        {
            var builder = new UriBuilder
            {
                Scheme = address.Scheme,
                Host = address.Host,
                Port = address.Port,
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
            if (ExchangeType != ExchangeType.FanOut)
                yield return $"{ExchangeTypeKey}={ExchangeType}";
            if (!string.IsNullOrWhiteSpace(InstanceId))
                yield return $"{InstanceIdKey}={InstanceId}";
        }
    }
}
