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
        readonly List<Action<ISendPipeConfigurator>> _configureSend;
        readonly ReceiveEndpointObservable _endpointObservers;
        readonly SingleThreadedDictionary<string, IKafkaProducerSpecification> _producers;
        readonly SendObservable _sendObservers;
        readonly SingleThreadedDictionary<string, IKafkaConsumerSpecification> _topics;
        IHeadersDeserializer _headersDeserializer;
        IHeadersSerializer _headersSerializer;
        bool _isHostConfigured;
        Action<IClient, string> _oAuthBearerTokenRefreshHandler;
        IKafkaSerializerFactory _serializerFactory;

        public KafkaFactoryConfigurator(ClientConfig clientConfig)
        {
            _clientConfig = clientConfig;
            _topics = new SingleThreadedDictionary<string, IKafkaConsumerSpecification>();
            _producers = new SingleThreadedDictionary<string, IKafkaProducerSpecification>();
            _endpointObservers = new ReceiveEndpointObservable();
            _sendObservers = new SendObservable();
            _serializerFactory = new DefaultKafkaSerializerFactory();
            _configureSend = new List<Action<ISendPipeConfigurator>>();

            SetHeadersDeserializer(DictionaryHeadersSerialize.Deserializer);
            SetHeadersSerializer(DictionaryHeadersSerialize.Serializer);

            _clientSupervisor = new Recycle<IClientContextSupervisor>(() => new ClientContextSupervisor(_clientConfig));
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

        public void OAuthBearerTokenRefreshHandler(Action<IClient, string> handler)
        {
            _oAuthBearerTokenRefreshHandler = handler ?? throw new ArgumentNullException(nameof(handler));
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
            if (!_topics.TryAdd(specification.EndpointName, _ => specification))
                throw new ConfigurationException($"A topic consumer with the same key was already added: {specification.EndpointName}");
        }

        public void TopicEndpoint<TKey, TValue>(string topicName, ConsumerConfig consumerConfig,
            Action<IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>> configure)
            where TValue : class
        {
            var specification = CreateSpecification(topicName, consumerConfig, configure);
            if (!_topics.TryAdd(specification.EndpointName, _ => specification))
                throw new ConfigurationException($"A topic consumer with the same key was already added: {specification.EndpointName}");
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

            var added = _producers.TryAdd(topicName, topic => CreateSpecification(topic, producerConfig, configure));

            if (!added)
                throw new ConfigurationException($"A topic producer with the same key was already added: {topicName}");
        }

        public void SetHeadersDeserializer(IHeadersDeserializer deserializer)
        {
            _headersDeserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        }

        public void SetHeadersSerializer(IHeadersSerializer serializer)
        {
            _headersSerializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public void SetSerializationFactory(IKafkaSerializerFactory factory)
        {
            _serializerFactory = factory ?? throw new ArgumentNullException(nameof(factory));
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
            _configureSend.Add(callback ?? throw new ArgumentNullException(nameof(callback)));
        }

        public KafkaSendTransportContext<TKey, TValue> CreateSendTransportContext<TKey, TValue>(IBusInstance busInstance, string topic)
            where TValue : class
        {
            if (!_producers.TryGetValue(topic, out var spec))
                spec = CreateSpecification<TKey, TValue>(topic);

            if (spec is IKafkaProducerSpecification<TKey, TValue> specification)
                return specification.CreateSendTransportContext(busInstance);

            throw new ConfigurationException($"Producer for topic: {topic} is not configured for ${typeof(Message<TKey, TValue>).Name} message");
        }

        public IKafkaProducerSpecification CreateSpecification<TKey, TValue>(string topicName,
            Action<IKafkaProducerConfigurator<TKey, TValue>> configure = null)
            where TValue : class
        {
            return CreateSpecification(topicName, new ProducerConfig(), configure);
        }

        public IKafkaProducerSpecification CreateSpecification<TKey, TValue>(string topicName, ProducerConfig producerConfig,
            Action<IKafkaProducerConfigurator<TKey, TValue>> configure = null)
            where TValue : class
        {
            var configurator = new KafkaProducerSpecification<TKey, TValue>(this, producerConfig, topicName, _oAuthBearerTokenRefreshHandler, _configureSend);
            configurator.SetHeadersSerializer(_headersSerializer);
            configurator.SetKeySerializer(_serializerFactory.GetSerializer<TKey>());
            configurator.SetValueSerializer(_serializerFactory.GetSerializer<TValue>());
            configure?.Invoke(configurator);

            configurator.ConnectSendObserver(_sendObservers);
            return configurator;
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

            var specification =
                new KafkaConsumerSpecification<TKey, TValue>(this, consumerConfig, topicName, _headersDeserializer, _serializerFactory, configure,
                    _oAuthBearerTokenRefreshHandler);
            specification.ConnectReceiveEndpointObserver(_endpointObservers);
            return specification;
        }

        public IReadOnlyDictionary<string, string> Configuration => _clientConfig.ToDictionary(x => x.Key, x => x.Value);

        public IClientContextSupervisor ClientContextSupervisor => _clientSupervisor.Supervisor;

        public IKafkaRider Build(IRiderRegistrationContext context, IBusInstance busInstance)
        {
            ConnectSendObserver(busInstance.HostConfiguration.SendObservers);

            var endpoints = new ReceiveEndpointCollection();
            foreach (var specification in _topics.Values)
                endpoints.Add(specification.EndpointName, specification.CreateReceiveEndpoint(busInstance));

            return new KafkaRider(this, busInstance, endpoints, context);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (string.IsNullOrEmpty(_clientConfig.BootstrapServers))
                yield return this.Failure("BootstrapServers", "should not be empty. Please use cfg.Host() to configure it");

            foreach (var result in _topics.Values.SelectMany(x => x.Validate()))
                yield return result;

            foreach (var result in _producers.Values.SelectMany(x => x.Validate()))
                yield return result;
        }

        public IBusInstanceSpecification Build(IRiderRegistrationContext context)
        {
            return new KafkaBusInstanceSpecification(context, this);
        }
    }
}
