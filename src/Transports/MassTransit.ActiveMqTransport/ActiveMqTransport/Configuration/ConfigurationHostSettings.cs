namespace MassTransit.ActiveMqTransport.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Apache.NMS;


    public class ConfigurationHostSettings :
        ActiveMqHostSettings
    {
        // ActiveMQ Failover connection parameters https://activemq.apache.org/components/classic/documentation/failover-transport-reference
        static readonly HashSet<string> _failoverArguments =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "backup",
                "initialReconnectDelay",
                "maxCacheSize",
                "maxReconnectAttempts",
                "maxReconnectDelay",
                "randomize",
                "reconnectDelayExponent",
                "reconnectSupported",
                "startupMaxReconnectAttempts",
                "timeout",
                "trackMessages",
                "updateURIsSupported",
                "updateURIsURL",
                "useExponentialBackOff",
                "warnAfterReconnectAttempts",
                "ha",
                "reconnectAttempts",
                "priorityBackup",
                "priorityURIs"
            };

        readonly Lazy<Uri> _brokerAddress;
        readonly Lazy<Uri> _hostAddress;

        public ConfigurationHostSettings(Uri address)
        {
            var hostAddress = new ActiveMqHostAddress(address);

            Host = hostAddress.Host;
            Port = hostAddress.Port ?? 61616;

            Username = "";
            Password = "";

            if (!string.IsNullOrEmpty(address.UserInfo))
            {
                var parts = address.UserInfo.Split(':');
                Username = parts[0];

                if (parts.Length >= 2)
                    Password = parts[1];
            }

            TransportOptions = new Dictionary<string, string> { { "wireFormat.tightEncodingEnabled", "true" } };

            _hostAddress = new Lazy<Uri>(FormatHostAddress);
            _brokerAddress = new Lazy<Uri>(FormatBrokerAddress);
        }

        public string[] FailoverHosts { get; set; }
        public Dictionary<string, string> TransportOptions { get; }

        public string Host { get; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; }

        public Uri HostAddress => _hostAddress.Value;
        public Uri BrokerAddress => _brokerAddress.Value;

        public IConnection CreateConnection()
        {
            var factory = new NMSConnectionFactory(BrokerAddress);

            return factory.ConnectionFactory.CreateConnection(Username, Password);
        }

        Uri FormatHostAddress()
        {
            return new ActiveMqHostAddress(Host, Port, "/");
        }

        Uri FormatBrokerAddress()
        {
            var scheme = UseSsl ? "ssl" : "tcp";

            // create broker URI: http://activemq.apache.org/nms/activemq-uri-configuration.html
            if (FailoverHosts?.Length > 0)
            {
                //filter only parameters which are not failover parameters
                var failoverServerPart = GetQueryString(kv => !IsFailoverArgument(kv.Key));
                var failoverPart = string.Join(",", FailoverHosts
                    .Select(failoverHost => new UriBuilder
                        {
                            Scheme = scheme,
                            Host = failoverHost,
                            Port = Port,
                            Query = failoverServerPart
                        }.Uri.ToString()
                    ));
                //filter failover parameters only. Apache.NMS.ActiveMQ requires prefix "transport." for failover parameters
                var failoverQueryPart = GetQueryString(kv => IsFailoverArgument(kv.Key), "transport.");
                return new Uri($"activemq:failover:({failoverPart}){failoverQueryPart}");
            }

            var queryPart = GetQueryString(_ => true);
            var uri = new Uri($"activemq:{scheme}://{Host}:{Port}{queryPart}");
            return uri;
        }

        string GetQueryString(Func<KeyValuePair<string, string>, bool> predicate, string prefix = "")
        {
            if (TransportOptions.Count == 0)
                return "";

            var queryString = string.Join("&", TransportOptions.Where(predicate).Select(pair => $"{prefix}{pair.Key}={pair.Value}"));

            return $"?{queryString}";
        }

        public override string ToString()
        {
            return new UriBuilder
            {
                Scheme = UseSsl ? "ssl" : "tcp",
                Host = Host,
                Port = Port
            }.Uri.ToString();
        }

        static bool IsFailoverArgument(string key)
        {
            return key.StartsWith("nested.", StringComparison.OrdinalIgnoreCase) || _failoverArguments.Contains(key);
        }
    }
}
