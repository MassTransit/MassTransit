namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Topology;


    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
    public readonly struct RabbitMqEndpointAddress
    {
        const string AutoDeleteKey = "autodelete";
        const string DurableKey = "durable";
        const string TemporaryKey = "temporary";
        const string ExchangeTypeKey = "type";
        const string BindQueueKey = "bind";
        const string QueueNameKey = "queue";
        const string AlternateExchangeKey = "alternateexchange";
        const string BindExchangeKey = "bindexchange";
        const string DelayedTypeKey = "delayedtype";

        const string DelayedMessageExchangeType = "x-delayed-message";

        public readonly string Scheme;
        public readonly string Host;
        public readonly int? Port;
        public readonly string VirtualHost;

        public readonly string Name;
        public readonly string ExchangeType;
        public readonly bool Durable;
        public readonly bool AutoDelete;
        public readonly bool BindToQueue;
        public readonly string QueueName;
        public readonly string DelayedType;
        public readonly string[] BindExchanges;
        public readonly string AlternateExchange;

        public RabbitMqEndpointAddress(Uri hostAddress, Uri address)
        {
            Scheme = default;
            Host = default;
            Port = default;
            VirtualHost = default;

            Durable = true;
            AutoDelete = false;
            ExchangeType = RabbitMQ.Client.ExchangeType.Fanout;

            BindToQueue = false;
            QueueName = default;
            DelayedType = default;
            AlternateExchange = default;
            BindExchanges = default;

            var scheme = address.Scheme.ToLowerInvariant();
            if (scheme.EndsWith("s"))
                Port = 5671;

            switch (scheme)
            {
                case "rabbitmqs":
                case "amqps":
                case "rabbitmq":
                case "amqp":
                    Scheme = address.Scheme;
                    Host = address.Host;
                    Port = !address.IsDefaultPort ? address.Port : scheme.EndsWith("s") ? 5671 : 5672;

                    ParsePath(address.AbsolutePath, out VirtualHost, out Name);
                    break;

                case "queue":
                    ParseLeft(hostAddress, out Scheme, out Host, out Port, out VirtualHost);

                    Name = address.AbsolutePath;
                    BindToQueue = true;
                    break;

                case "exchange":
                    ParseLeft(hostAddress, out Scheme, out Host, out Port, out VirtualHost);

                    Name = address.AbsolutePath;
                    break;

                default:
                    throw new ArgumentException($"The address scheme is not supported: {address.Scheme}", nameof(address));
            }

            if (Name == "*")
                Name = NewId.Next().ToString("NS");

            RabbitMqEntityNameValidator.Validator.ThrowIfInvalidEntityName(Name);

            string query = address.Query?.TrimStart('?');
            if (!string.IsNullOrWhiteSpace(query))
            {
                HashSet<string> bindExchanges = new HashSet<string>();

                var parameters = query.Split('&').Select(x => x.Split('=')).Select(x => (x.First().ToLowerInvariant(), x.Skip(1).FirstOrDefault()));

                foreach ((string key, string value) in parameters)
                {
                    switch (key)
                    {
                        case TemporaryKey when bool.TryParse(value, out bool result):
                            AutoDelete = result;
                            Durable = !result;
                            break;

                        case DurableKey when bool.TryParse(value, out bool result):
                            Durable = result;
                            break;

                        case AutoDeleteKey when bool.TryParse(value, out bool result):
                            AutoDelete = result;
                            break;

                        case ExchangeTypeKey when !string.IsNullOrWhiteSpace(value):
                            ExchangeType = value;
                            break;

                        case BindQueueKey when bool.TryParse(value, out bool result):
                            BindToQueue = result;
                            break;

                        case QueueNameKey when !string.IsNullOrWhiteSpace(value):
                            QueueName = Uri.UnescapeDataString(value);
                            break;

                        case DelayedTypeKey when !string.IsNullOrWhiteSpace(value):
                            DelayedType = value;
                            ExchangeType = DelayedMessageExchangeType;
                            break;

                        case AlternateExchangeKey when !string.IsNullOrWhiteSpace(value):
                            AlternateExchange = value;
                            break;

                        case BindExchangeKey when !string.IsNullOrWhiteSpace(value):
                            bindExchanges.Add(value);
                            break;
                    }
                }

                if (bindExchanges.Count > 0)
                    BindExchanges = bindExchanges.ToArray();
            }
        }

        public RabbitMqEndpointAddress(Uri hostAddress, string exchangeName, string exchangeType = default, bool durable = true, bool autoDelete = false,
            bool bindToQueue = false, string queueName = default, string delayedType = default, string[] bindExchanges = default,
            string alternateExchange = default)
        {
            ParseLeft(hostAddress, out Scheme, out Host, out Port, out VirtualHost);

            Name = exchangeName;
            ExchangeType = exchangeType ?? RabbitMQ.Client.ExchangeType.Fanout;

            Durable = durable;
            AutoDelete = autoDelete;
            BindToQueue = bindToQueue;
            QueueName = queueName;
            DelayedType = delayedType;
            BindExchanges = bindExchanges;
            AlternateExchange = alternateExchange;
        }

        RabbitMqEndpointAddress(string scheme, string host, int? port, string virtualHost, string name, string exchangeType, bool durable,
            bool autoDelete, bool bindToQueue, string queueName, string delayedType, string[] bindExchanges, string alternateExchange)
        {
            Scheme = scheme;
            Host = host;
            Port = port;
            VirtualHost = virtualHost;
            Name = name;
            ExchangeType = exchangeType;
            Durable = durable;
            AutoDelete = autoDelete;
            BindToQueue = bindToQueue;
            QueueName = queueName;
            DelayedType = delayedType;
            BindExchanges = bindExchanges;
            AlternateExchange = alternateExchange;
        }

        public RabbitMqEndpointAddress GetDelayAddress()
        {
            string name = $"{Name}_delay";

            return new RabbitMqEndpointAddress(Scheme, Host, Port, VirtualHost, name, DelayedMessageExchangeType, Durable, AutoDelete, BindToQueue,
                QueueName, ExchangeType, BindExchanges, AlternateExchange);
        }

        static void ParsePath(string path, out string virtualHost, out string name)
        {
            int split = path.LastIndexOf('/');
            if (split > 0)
            {
                virtualHost = Uri.UnescapeDataString(path.Substring(1, split - 1));
                name = path.Substring(split + 1);
            }
            else
            {
                virtualHost = "/";
                name = path.Substring(1);
            }
        }

        static void ParseLeft(Uri address, out string scheme, out string host, out int? port, out string virtualHost)
        {
            var hostAddress = new RabbitMqHostAddress(address);
            scheme = hostAddress.Scheme;
            host = hostAddress.Host;
            port = hostAddress.Port;
            virtualHost = hostAddress.VirtualHost;
        }

        public static implicit operator Uri(in RabbitMqEndpointAddress address)
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
                    ? $"/{address.Name}"
                    : $"/{Uri.EscapeDataString(address.VirtualHost)}/{address.Name}"
            };

            builder.Query += string.Join("&", address.GetQueryStringOptions());

            return builder.Uri;
        }

        Uri DebuggerDisplay => this;

        IEnumerable<string> GetQueryStringOptions()
        {
            if (!Durable && AutoDelete)
                yield return $"{TemporaryKey}=true";
            else if (!Durable)
                yield return $"{DurableKey}=false";
            else if (AutoDelete)
                yield return $"{AutoDeleteKey}=true";

            var noDelayedType = string.IsNullOrWhiteSpace(DelayedType);

            if (ExchangeType != RabbitMQ.Client.ExchangeType.Fanout && noDelayedType)
                yield return $"{ExchangeTypeKey}={ExchangeType}";

            if (BindToQueue)
                yield return $"{BindQueueKey}=true";
            if (!string.IsNullOrWhiteSpace(QueueName))
                yield return $"{QueueNameKey}={Uri.EscapeDataString(QueueName)}";

            if (!noDelayedType)
                yield return $"{DelayedTypeKey}={DelayedType}";

            if (!string.IsNullOrWhiteSpace(AlternateExchange))
                yield return $"{AlternateExchangeKey}={AlternateExchange}";

            if (BindExchanges != null)
                foreach (var binding in BindExchanges)
                    yield return $"{BindExchangeKey}={binding}";
        }
    }
}
