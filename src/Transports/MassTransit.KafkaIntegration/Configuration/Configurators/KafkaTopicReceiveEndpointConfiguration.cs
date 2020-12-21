namespace MassTransit.KafkaIntegration.Configurators
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Confluent.Kafka;
    using Filters;
    using GreenPipes;
    using GreenPipes.Configurators;
    using MassTransit.Registration;
    using Serializers;
    using Transport;
    using Transports;


    public class KafkaTopicReceiveEndpointConfiguration<TKey, TValue> :
        ReceiverConfiguration,
        ReceiveSettings,
        IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>
        where TValue : class
    {
        readonly IBusInstance _busInstance;
        readonly ConsumerConfig _consumerConfig;
        readonly IReceiveEndpointConfiguration _endpointConfiguration;
        IHeadersDeserializer _headersDeserializer;
        IDeserializer<TKey> _keyDeserializer;
        Action<IConsumer<TKey, TValue>, CommittedOffsets> _offsetsCommittedHandler;
        IDeserializer<TValue> _valueDeserializer;
        readonly PipeConfigurator<IKafkaConsumerContext<TKey, TValue>> _consumerConfigurator;

        public KafkaTopicReceiveEndpointConfiguration(ConsumerConfig consumerConfig, string topic, IBusInstance busInstance,
            IReceiveEndpointConfiguration endpointConfiguration, IHeadersDeserializer headersDeserializer)
            : base(endpointConfiguration)
        {
            _busInstance = busInstance;
            _endpointConfiguration = endpointConfiguration;
            _consumerConfig = consumerConfig;
            Topic = topic;

            SetValueDeserializer(new MassTransitJsonDeserializer<TValue>());
            SetKeyDeserializer(new MassTransitJsonDeserializer<TKey>());
            SetHeadersDeserializer(headersDeserializer);

            CheckpointInterval = TimeSpan.FromMinutes(1);
            CheckpointMessageCount = 5000;
            ConcurrencyLimit = 1;

            _consumerConfigurator = new PipeConfigurator<IKafkaConsumerContext<TKey, TValue>>();
        }

        public AutoOffsetReset? AutoOffsetReset
        {
            set => _consumerConfig.AutoOffsetReset = value;
        }

        public string GroupInstanceId
        {
            set => _consumerConfig.GroupInstanceId = value;
        }

        public TimeSpan CheckpointInterval { set; get; }

        public int ConcurrencyLimit { set; get; }

        public string Topic { get; }

        public ushort CheckpointMessageCount { set; get; }

        public PartitionAssignmentStrategy? PartitionAssignmentStrategy
        {
            set => _consumerConfig.PartitionAssignmentStrategy = value;
        }

        public TimeSpan? SessionTimeout
        {
            set => _consumerConfig.SessionTimeoutMs = value?.Milliseconds;
        }

        public TimeSpan? HeartbeatInterval
        {
            set => _consumerConfig.HeartbeatIntervalMs = value?.Milliseconds;
        }

        public string GroupProtocolType
        {
            set => _consumerConfig.GroupProtocolType = value;
        }

        public TimeSpan? CoordinatorQueryInterval
        {
            set => _consumerConfig.CoordinatorQueryIntervalMs = value?.Milliseconds;
        }

        public TimeSpan? MaxPollInterval
        {
            set => _consumerConfig.MaxPollIntervalMs = value?.Milliseconds;
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

        public void SetOffsetsCommittedHandler(Action<IConsumer<TKey, TValue>, CommittedOffsets> offsetsCommittedHandler)
        {
            if (_offsetsCommittedHandler != null)
                throw new InvalidOperationException("Offset committed handler may not be specified more than once.");

            _offsetsCommittedHandler = offsetsCommittedHandler ?? throw new ArgumentNullException(nameof(offsetsCommittedHandler));
        }

        public override IEnumerable<ValidationResult> Validate()
        {
            if (_headersDeserializer == null)
                yield return this.Failure("HeadersDeserializer", "should not be null");

            foreach (var result in base.Validate())
                yield return result;
        }

        public IReceiveEndpointControl Build()
        {
            IKafkaReceiveEndpointContext<TKey, TValue> CreateContext()
            {
                var builder = new KafkaReceiveEndpointBuilder<TKey, TValue>(_busInstance, _endpointConfiguration, _headersDeserializer, this,
                    CreateConsumerBuilder);
                foreach (var specification in Specifications)
                    specification.Configure(builder);

                return builder.CreateReceiveEndpointContext();
            }

            IKafkaReceiveEndpointContext<TKey, TValue> context = CreateContext();

            _consumerConfigurator.UseFilter(new KafkaConsumerFilter<TKey, TValue>(context));

            IPipe<IKafkaConsumerContext<TKey, TValue>> consumerPipe = _consumerConfigurator.Build();

            var transport = new ReceiveTransport<IKafkaConsumerContext<TKey, TValue>>(_busInstance.HostConfiguration, context,
                () => context.ConsumerContextSupervisor, consumerPipe);

            return new ReceiveEndpoint(transport, context);
        }

        ConsumerBuilder<TKey, TValue> CreateConsumerBuilder()
        {
            ConsumerBuilder<TKey, TValue> consumerBuilder = new ConsumerBuilder<TKey, TValue>(_consumerConfig)
                .SetValueDeserializer(_valueDeserializer);

            if (_keyDeserializer != null)
                consumerBuilder.SetKeyDeserializer(_keyDeserializer);
            if (_offsetsCommittedHandler != null)
                consumerBuilder.SetOffsetsCommittedHandler(_offsetsCommittedHandler);

            return consumerBuilder;
        }
    }
}
