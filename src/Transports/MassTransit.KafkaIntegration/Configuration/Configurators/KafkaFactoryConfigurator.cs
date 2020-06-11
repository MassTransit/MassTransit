namespace MassTransit.KafkaIntegration.Configurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Confluent.Kafka;
    using GreenPipes;
    using MassTransit.Registration;
    using Pipeline.Observables;
    using Riders;
    using Serializers;
    using Specifications;


    public class KafkaFactoryConfigurator :
        IKafkaFactoryConfigurator
    {
        readonly ClientConfig _clientConfig;
        readonly ReceiveEndpointObservable _endpointObservers;
        readonly RiderObservable _observers;
        readonly List<IKafkaProducerSpecification> _producers;
        readonly SendObservable _sendObservers;
        readonly List<IKafkaConsumerSpecification> _topics;
        Action<ISendPipeConfigurator> _configureSend;
        IHeadersDeserializer _headersDeserializer;
        IHeadersSerializer _headersSerializer;
        bool _isHostConfigured;

        public KafkaFactoryConfigurator(ClientConfig clientConfig)
        {
            _clientConfig = clientConfig;
            _topics = new List<IKafkaConsumerSpecification>();
            _producers = new List<IKafkaProducerSpecification>();
            _observers = new RiderObservable();
            _endpointObservers = new ReceiveEndpointObservable();
            _sendObservers = new SendObservable();

            SetHeadersDeserializer(DictionaryHeadersSerialize.Deserializer);
            SetHeadersSerializer(DictionaryHeadersSerialize.Serializer);
        }

        public void Host(IEnumerable<string> servers, Action<IKafkaHostConfigurator> configure)
        {
            if (servers == null || !servers.Any())
                throw new ArgumentException(nameof(servers));

            if (_isHostConfigured)
                throw new ConfigurationException("Host may not be specified more than once.");
            _isHostConfigured = true;

            const string serverSeparator = ",";

            _clientConfig.BootstrapServers = string.Join(serverSeparator, servers);
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

        public void TopicEndpoint<TKey, TValue>(string topicName, string groupId, Action<IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            if (string.IsNullOrEmpty(groupId))
                throw new ArgumentException(groupId);

            TopicEndpoint(topicName, new ConsumerConfig(_clientConfig) {GroupId = groupId}, configure);
        }

        public void TopicEndpoint<TKey, TValue>(string topicName, ConsumerConfig consumerConfig,
            Action<IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            if (consumerConfig == null)
                throw new ArgumentNullException(nameof(consumerConfig));

            consumerConfig.AutoCommitIntervalMs = null;
            consumerConfig.EnableAutoCommit = false;

            var topic = new KafkaConsumerSpecification<TKey, TValue>(consumerConfig, topicName, _headersDeserializer, configure);
            topic.ConnectReceiveEndpointObserver(_endpointObservers);
            _topics.Add(topic);
        }

        void IKafkaFactoryConfigurator.TopicProducer<TKey, TValue>(string topicName, Action<IKafkaProducerConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            this.TopicProducer(topicName, new ProducerConfig(_clientConfig), configure);
        }

        void IKafkaFactoryConfigurator.TopicProducer<TKey, TValue>(string topicName, ProducerConfig producerConfig,
            Action<IKafkaProducerConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            if (string.IsNullOrWhiteSpace(topicName))
                throw new ArgumentException(nameof(topicName));
            if (producerConfig == null)
                throw new ArgumentNullException(nameof(producerConfig));

            var configurator = new KafkaProducerSpecification<TKey, TValue>(producerConfig, topicName, _headersSerializer);
            configure?.Invoke(configurator);

            configurator.ConnectSendObserver(_sendObservers);
            if (_configureSend != null)
                configurator.ConfigureSend(_configureSend);

            _producers.Add(configurator);
        }

        public void SetHeadersDeserializer(IHeadersDeserializer deserializer)
        {
            _headersDeserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        }

        public void SetHeadersSerializer(IHeadersSerializer serializer)
        {
            _headersSerializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
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

        public ConnectHandle ConnectRiderObserver(IRiderObserver observer)
        {
            return _observers.Connect(observer);
        }

        public ConnectHandle ConnectReceiveEndpointObserver(IReceiveEndpointObserver observer)
        {
            return _endpointObservers.Connect(observer);
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _sendObservers.Connect(observer);
        }

        public void ConfigureSend(Action<ISendPipeConfigurator> callback)
        {
            _configureSend = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        public IBusInstanceSpecification Build()
        {
            return new KafkaBusInstanceSpecification(_topics, _producers, _observers);
        }
    }
}
