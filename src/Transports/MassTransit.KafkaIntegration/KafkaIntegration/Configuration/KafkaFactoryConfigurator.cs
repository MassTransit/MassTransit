namespace MassTransit.KafkaIntegration.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Confluent.Kafka;
    using MassTransit.Configuration;
    using Observables;
    using Serializers;
    using Transports;
    using Util;


    public class KafkaFactoryConfigurator :
        IKafkaFactoryConfigurator,
        IKafkaHostConfiguration
    {
        readonly ClientConfig _clientConfig;
        readonly Recycle<IClientContextSupervisor> _clientSupervisor;
        readonly ReceiveEndpointObservable _endpointObservers;
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
            _endpointObservers = new ReceiveEndpointObservable();
            _sendObservers = new SendObservable();

            SetHeadersDeserializer(DictionaryHeadersSerialize.Deserializer);
            SetHeadersSerializer(DictionaryHeadersSerialize.Serializer);

            _clientSupervisor = new Recycle<IClientContextSupervisor>(() => new ClientContextSupervisor(_clientConfig, _producers));
        }

        public void Host(IReadOnlyList<string> servers, Action<IKafkaHostConfigurator> configure)
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
            var specification = CreateSpecification(topicName, groupId, configure);
            _topics.Add(specification);
        }

        public void TopicEndpoint<TKey, TValue>(string topicName, ConsumerConfig consumerConfig,
            Action<IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            var specification = CreateSpecification(topicName, consumerConfig, configure);
            _topics.Add(specification);
        }

        void IKafkaFactoryConfigurator.TopicProducer<TKey, TValue>(string topicName, Action<IKafkaProducerConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            this.TopicProducer(topicName, new ProducerConfig(), configure);
        }

        void IKafkaFactoryConfigurator.TopicProducer<TKey, TValue>(string topicName, ProducerConfig producerConfig,
            Action<IKafkaProducerConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            if (string.IsNullOrWhiteSpace(topicName))
                throw new ArgumentException(nameof(topicName));
            if (producerConfig == null)
                throw new ArgumentNullException(nameof(producerConfig));

            var configurator = new KafkaProducerSpecification<TKey, TValue>(this, producerConfig, topicName, _headersSerializer);
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

        public TimeSpan? TopicMetadataRefreshInterval
        {
            set => _clientConfig.TopicMetadataRefreshIntervalMs = (int)value?.TotalMilliseconds;
        }

        public TimeSpan? MetadataMaxAge
        {
            set => _clientConfig.MetadataMaxAgeMs = (int)value?.TotalMilliseconds;
        }

        public TimeSpan? TopicMetadataRefreshFastInterval
        {
            set => _clientConfig.TopicMetadataRefreshFastIntervalMs = (int)value?.TotalMilliseconds;
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
            set => _clientConfig.ReconnectBackoffMs = (int)value?.TotalMilliseconds;
        }

        public TimeSpan? ReconnectBackoffMax
        {
            set => _clientConfig.ReconnectBackoffMaxMs = (int)value?.TotalMilliseconds;
        }

        public TimeSpan? StatisticsInterval
        {
            set => _clientConfig.StatisticsIntervalMs = (int)value?.TotalMilliseconds;
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

        public IKafkaConsumerSpecification CreateSpecification<TKey, TValue>(string topicName, string groupId,
            Action<IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            if (string.IsNullOrEmpty(groupId))
                throw new ArgumentException(groupId);

            return CreateSpecification(topicName, new ConsumerConfig { GroupId = groupId }, configure);
        }

        public IKafkaConsumerSpecification CreateSpecification<TKey, TValue>(string topicName, ConsumerConfig consumerConfig,
            Action<IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            if (consumerConfig == null)
                throw new ArgumentNullException(nameof(consumerConfig));

            consumerConfig.AutoCommitIntervalMs = null;
            consumerConfig.EnableAutoCommit = false;

            var specification = new KafkaConsumerSpecification<TKey, TValue>(this, consumerConfig, topicName, _headersDeserializer, configure);
            specification.ConnectReceiveEndpointObserver(_endpointObservers);
            return specification;
        }

        public IReadOnlyDictionary<string, string> Configuration => _clientConfig.ToDictionary(x => x.Key, x => x.Value);

        public IClientContextSupervisor ClientContextSupervisor => _clientSupervisor.Supervisor;

        public IKafkaRider Build(IRiderRegistrationContext context, IBusInstance busInstance)
        {
            ConnectSendObserver(busInstance.HostConfiguration.SendObservers);

            var endpoints = new ReceiveEndpointCollection();
            foreach (var topic in _topics)
                endpoints.Add(topic.EndpointName, topic.CreateReceiveEndpoint(busInstance));

            var topicProducerProvider = new TopicProducerProvider(busInstance, this);

            return new KafkaRider(this, busInstance, endpoints, topicProducerProvider, context);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(_clientConfig.BootstrapServers))
                yield return this.Failure("BootstrapServers", "should not be empty. Please use cfg.Host() to configure it");

            foreach (KeyValuePair<string, IKafkaConsumerSpecification[]> kv in _topics.GroupBy(x => x.EndpointName)
                         .ToDictionary(x => x.Key, x => x.ToArray()))
            {
                if (kv.Value.Length > 1)
                    yield return this.Failure($"Topic: {kv.Key} was added more than once.");

                foreach (var result in kv.Value.SelectMany(x => x.Validate()))
                    yield return result;
            }

            foreach (var result in _producers.SelectMany(x => x.Validate()))
                yield return result;
        }

        public IBusInstanceSpecification Build(IRiderRegistrationContext context)
        {
            return new KafkaBusInstanceSpecification(context, this);
        }
    }
}
