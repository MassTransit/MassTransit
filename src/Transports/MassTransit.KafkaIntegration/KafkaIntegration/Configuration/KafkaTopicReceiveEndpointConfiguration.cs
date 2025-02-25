namespace MassTransit.KafkaIntegration.Configuration
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;
    using MassTransit.Configuration;
    using MassTransit.Middleware;
    using Middleware;
    using Serializers;
    using Transports;


    public class KafkaTopicReceiveEndpointConfiguration<TKey, TValue> :
        ReceiveEndpointConfiguration,
        ReceiveSettings,
        IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>
        where TValue : class
    {
        readonly IBusInstance _busInstance;
        readonly ConsumerConfig _consumerConfig;
        readonly PipeConfigurator<ConsumerContext> _consumerConfigurator;
        readonly IReceiveEndpointConfiguration _endpointConfiguration;
        readonly IKafkaHostConfiguration _hostConfiguration;
        readonly Action<IClient, string> _oAuthBearerTokenRefreshHandler;
        readonly IOptionsSet _options;
        IHeadersDeserializer _headersDeserializer;
        IDeserializer<TKey> _keyDeserializer;
        Action<CommittedOffsets> _offsetsCommittedHandler;
        Action<string> _statisticsHandler;
        IDeserializer<TValue> _valueDeserializer;

        public KafkaTopicReceiveEndpointConfiguration(IKafkaHostConfiguration hostConfiguration, ConsumerConfig consumerConfig, string topic,
            IBusInstance busInstance, IReceiveEndpointConfiguration endpointConfiguration, Action<IClient, string> oAuthBearerTokenRefreshHandler)
            : base(busInstance.HostConfiguration, endpointConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _busInstance = busInstance;
            _endpointConfiguration = endpointConfiguration;
            _oAuthBearerTokenRefreshHandler = oAuthBearerTokenRefreshHandler;
            _consumerConfig = consumerConfig;
            _options = new OptionsSet();
            Topic = topic;

            CheckpointInterval = TimeSpan.FromMinutes(1);
            CheckpointMessageCount = 5000;
            MessageLimit = 10000;
            ConcurrentMessageLimit = 1;
            ConcurrentDeliveryLimit = 1;
            ConcurrentConsumerLimit = 1;
            PrefetchCount = Math.Max(1000, CheckpointMessageCount / 10);

            _consumerConfigurator = new PipeConfigurator<ConsumerContext>();

            // https://github.com/confluentinc/confluent-kafka-dotnet/blob/0e6bc8be05988f0cafacfe2b71aa8950aabe8cb5/src/Confluent.Kafka/ConsumerBuilder.cs#L387
            Offset = Confluent.Kafka.Offset.Unset;

            PublishFaults = false;

            this.DiscardFaultedMessages();
            this.DiscardSkippedMessages();
        }

        public override Uri HostAddress => _endpointConfiguration.HostAddress;

        public AutoOffsetReset? AutoOffsetReset
        {
            set => _consumerConfig.AutoOffsetReset = value;
        }

        public string GroupInstanceId
        {
            set => _consumerConfig.GroupInstanceId = value;
        }

        public PartitionAssignmentStrategy? PartitionAssignmentStrategy
        {
            set => _consumerConfig.PartitionAssignmentStrategy = value;
        }

        public TimeSpan? SessionTimeout
        {
            set => _consumerConfig.SessionTimeoutMs = (int?)value?.TotalMilliseconds;
        }

        public TimeSpan? HeartbeatInterval
        {
            set => _consumerConfig.HeartbeatIntervalMs = (int?)value?.TotalMilliseconds;
        }

        public string GroupProtocolType
        {
            set => _consumerConfig.GroupProtocolType = value;
        }

        public TimeSpan? CoordinatorQueryInterval
        {
            set => _consumerConfig.CoordinatorQueryIntervalMs = (int?)value?.TotalMilliseconds;
        }

        public TimeSpan? MaxPollInterval
        {
            set => _consumerConfig.MaxPollIntervalMs = (int?)value?.TotalMilliseconds;
        }

        public bool? EnableAutoOffsetStore
        {
            set => _consumerConfig.EnableAutoOffsetStore = value;
        }

        public int? QueuedMinMessages
        {
            set => _consumerConfig.QueuedMinMessages = value;
        }

        public int? QueuedMaxMessagesKbytes
        {
            set => _consumerConfig.QueuedMaxMessagesKbytes = value;
        }

        public void ConfigureFetch(Action<IKafkaFetchConfigurator> configure)
        {
            var configurator = new KafkaFetchConfigurator(_consumerConfig);
            configure?.Invoke(configurator);
        }

        public void UseIsolationLevel(IsolationLevel isolationLevel)
        {
            _consumerConfig.IsolationLevel = isolationLevel;
        }

        public bool? EnablePartitionEof
        {
            set => _consumerConfig.EnablePartitionEof = value;
        }

        public bool? CheckCrcs
        {
            set => _consumerConfig.CheckCrcs = value;
        }

        public void SetKeyDeserializer(IDeserializer<TKey> deserializer)
        {
            _keyDeserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        }

        public void SetValueDeserializer(IDeserializer<TValue> deserializer)
        {
            _valueDeserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        }

        public void SetHeadersDeserializer(IHeadersDeserializer deserializer)
        {
            _headersDeserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        }

        public void SetOffsetsCommittedHandler(Action<CommittedOffsets> offsetsCommittedHandler)
        {
            _offsetsCommittedHandler = offsetsCommittedHandler ?? throw new ArgumentNullException(nameof(offsetsCommittedHandler));
        }

        public void SetStatisticsHandler(Action<string> statisticsHandler)
        {
            _statisticsHandler = statisticsHandler ?? throw new ArgumentNullException(nameof(statisticsHandler));
        }

        public void CreateIfMissing(Action<KafkaTopicOptions> configure)
        {
            if (_options.TryGetOptions<KafkaTopicOptions>(out _))
                throw new InvalidOperationException("Topic options has been already configured.");

            var options = new KafkaTopicOptions(Topic);
            configure?.Invoke(options);
            _options.Options(options);
        }

        public override Uri InputAddress => _endpointConfiguration.InputAddress;

        public ushort ConcurrentConsumerLimit { get; set; }
        public int ConcurrentDeliveryLimit { get; set; }
        public TimeSpan CheckpointInterval { set; get; }

        int ReceiveSettings.ConcurrentMessageLimit => Transport.GetConcurrentMessageLimit();

        public long Offset { get; set; }
        public string Topic { get; }
        public ushort MessageLimit { get; set; }

        public ushort CheckpointMessageCount { set; get; }

        public override IEnumerable<ValidationResult> Validate()
        {
            if (_headersDeserializer == null)
                yield return this.Failure("HeadersDeserializer", "should not be null");

            if (_keyDeserializer == null)
                yield return this.Failure("KeyDeserializer", "should not be null");

            if (_valueDeserializer == null)
                yield return this.Failure("ValueDeserializer", "should not be null");

            if (_options.TryGetOptions(out KafkaTopicOptions options))
            {
                foreach (var result in options.Validate())
                    yield return result;
            }

            foreach (var result in base.Validate())
                yield return result;
        }

        public override ReceiveEndpointContext CreateReceiveEndpointContext()
        {
            return CreateReceiveKafkaEndpointContext();
        }

        KafkaReceiveEndpointContext<TKey, TValue> CreateReceiveKafkaEndpointContext()
        {
            ConsumerBuilder<byte[], byte[]> CreateConsumerBuilder(int index)
            {
                var consumerConfig = _hostConfiguration.GetConsumerConfig(_consumerConfig);
                if (!string.IsNullOrWhiteSpace(consumerConfig.GroupInstanceId) && ConcurrentConsumerLimit > 1)
                    consumerConfig.GroupInstanceId += $"{index:D2}";

                ConsumerBuilder<byte[], byte[]> consumerBuilder = new ConsumerBuilder<byte[], byte[]>(consumerConfig)
                    .SetLogHandler((c, message) => _busInstance.HostConfiguration.ReceiveLogContext?.Debug?.Log(message.Message));

                if (_offsetsCommittedHandler != null)
                    consumerBuilder.SetOffsetsCommittedHandler((_, offsets) => _offsetsCommittedHandler(offsets));
                if (_oAuthBearerTokenRefreshHandler != null)
                    consumerBuilder.SetOAuthBearerTokenRefreshHandler(_oAuthBearerTokenRefreshHandler);
                if (_statisticsHandler != null)
                    consumerBuilder.SetStatisticsHandler((_, statistics) => _statisticsHandler(statistics));

                return consumerBuilder;
            }

            var builder = new KafkaReceiveEndpointBuilder<TKey, TValue>(_busInstance, _hostConfiguration, _consumerConfig.GroupId, this, this,
                _headersDeserializer, _keyDeserializer, _valueDeserializer, CreateConsumerBuilder);

            ApplySpecifications(builder);

            return builder.CreateReceiveEndpointContext();
        }

        public ReceiveEndpoint Build()
        {
            KafkaReceiveEndpointContext<TKey, TValue> context = CreateReceiveKafkaEndpointContext();

            if (_options.TryGetOptions(out KafkaTopicOptions options))
                _consumerConfigurator.UseFilter(new ConfigureKafkaTopologyFilter<TKey, TValue>(_hostConfiguration.Configuration, options));

            _consumerConfigurator.UseFilter(new ReceiveEndpointDependencyFilter<ConsumerContext>(context));
            _consumerConfigurator.UseFilter(new KafkaConsumerFilter<TKey, TValue>(context));

            IPipe<ConsumerContext> consumerPipe = _consumerConfigurator.Build();

            var transport = new ReceiveTransport<ConsumerContext>(_busInstance.HostConfiguration, context,
                () => context.ConsumerContextSupervisor, consumerPipe);

            return new ReceiveEndpoint(transport, context);
        }
    }
}
