namespace MassTransit.KafkaIntegration
{
    using System;
    using Configuration;
    using Configuration.Configurators;
    using Confluent.Kafka;


    public class KafkaAttachmentConfigurator :
        IKafkaAttachmentConfigurator
    {
        readonly ClientConfig _clientConfig;

        public KafkaAttachmentConfigurator()
        {
            _clientConfig = new ClientConfig();
        }

        public void Host(string servers, Action<IKafkaHostConfigurator> configure)
        {
            _clientConfig.BootstrapServers = servers;
            var configurator = new KafkaHostConfigurator(_clientConfig);
            configure?.Invoke(configurator);
        }

        public void ConfigureApi(Action<IKafkaApiConfigurator> configure)
        {
            var configurator = new KafkaApiConfigurator(_clientConfig);
            configure?.Invoke(configurator);
        }

        public void ConfigureSocket(Action<IKafkaSocketConfigurator> configure)
        {
            var configurator = new KafkaSocketConfigurator(_clientConfig);
            configure?.Invoke(configurator);
        }

        public Acks? Acks
        {
            set => _clientConfig.Acks = value;
        }

        public string ClientId
        {
            set => _clientConfig.ClientId = value;
        }

        public int? MessageMaxBytes
        {
            set => _clientConfig.MessageMaxBytes = value;
        }

        public int? MessageCopyMaxBytes
        {
            set => _clientConfig.MessageCopyMaxBytes = value;
        }

        public int? ReceiveMessageMaxBytes
        {
            set => _clientConfig.ReceiveMessageMaxBytes = value;
        }

        public int? MaxInFlight
        {
            set => _clientConfig.MaxInFlight = value;
        }

        public TimeSpan? MetadataRequestTimeout
        {
            set => _clientConfig.MetadataRequestTimeoutMs = value?.Milliseconds;
        }

        public TimeSpan? TopicMetadataRefreshInterval
        {
            set => _clientConfig.TopicMetadataRefreshIntervalMs = value?.Milliseconds;
        }

        public TimeSpan? MetadataMaxAge
        {
            set => _clientConfig.MetadataMaxAgeMs = value?.Milliseconds;
        }

        public TimeSpan? TopicMetadataRefreshFastInterval
        {
            set => _clientConfig.TopicMetadataRefreshFastIntervalMs = value?.Milliseconds;
        }

        public bool? TopicMetadataRefreshSparse
        {
            set => _clientConfig.TopicMetadataRefreshSparse = value;
        }

        public string TopicBlacklist
        {
            set => _clientConfig.TopicBlacklist = value;
        }

        public string Debug
        {
            set => _clientConfig.Debug = value;
        }

        public int? BrokerAddressTtl
        {
            set => _clientConfig.BrokerAddressTtl = value;
        }

        public BrokerAddressFamily? BrokerAddressFamily
        {
            set => _clientConfig.BrokerAddressFamily = value;
        }

        public TimeSpan? ReconnectBackoff
        {
            set => _clientConfig.ReconnectBackoffMs = value?.Milliseconds;
        }

        public TimeSpan? ReconnectBackoffMax
        {
            set => _clientConfig.ReconnectBackoffMaxMs = value?.Milliseconds;
        }

        public TimeSpan? StatisticsInterval
        {
            set => _clientConfig.StatisticsIntervalMs = value?.Milliseconds;
        }

        public bool? LogQueue
        {
            set => _clientConfig.LogQueue = value;
        }

        public bool? LogThreadName
        {
            set => _clientConfig.LogThreadName = value;
        }

        public bool? LogConnectionClose
        {
            set => _clientConfig.LogConnectionClose = value;
        }

        public int? InternalTerminationSignal
        {
            set => _clientConfig.InternalTerminationSignal = value;
        }

        public SecurityProtocol? SecurityProtocol
        {
            set => _clientConfig.SecurityProtocol = value;
        }

        public string PluginLibraryPaths
        {
            set => _clientConfig.PluginLibraryPaths = value;
        }

        public string ClientRack
        {
            set => _clientConfig.ClientRack = value;
        }
    }
}
