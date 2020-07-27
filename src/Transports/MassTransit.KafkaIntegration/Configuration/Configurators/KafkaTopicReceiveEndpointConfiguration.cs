namespace MassTransit.KafkaIntegration.Configurators
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Confluent.Kafka;
    using Context;
    using Contexts;
    using GreenPipes;
    using MassTransit.Registration;
    using Riders;
    using Serializers;
    using Transport;


    public class KafkaTopicReceiveEndpointConfiguration<TKey, TValue> :
        ReceiverConfiguration,
        IKafkaTopicReceiveEndpointConfigurator<TKey, TValue>
        where TValue : class
    {
        readonly IBusInstance _busInstance;
        readonly ConsumerConfig _consumerConfig;
        readonly IReceiveEndpointConfiguration _endpointConfiguration;
        readonly string _topic;
        TimeSpan _checkpointInterval;
        ushort _checkpointMessageCount;
        int _concurrencyLimit;
        IHeadersDeserializer _headersDeserializer;
        IDeserializer<TKey> _keyDeserializer;
        Action<IConsumer<TKey, TValue>, CommittedOffsets> _offsetsCommittedHandler;
        IDeserializer<TValue> _valueDeserializer;

        public KafkaTopicReceiveEndpointConfiguration(ConsumerConfig consumerConfig, string topic, IBusInstance busInstance,
            IReceiveEndpointConfiguration endpointConfiguration, IHeadersDeserializer headersDeserializer)
            : base(busInstance.HostConfiguration, endpointConfiguration)
        {
            _busInstance = busInstance;
            _endpointConfiguration = endpointConfiguration;
            _consumerConfig = consumerConfig;
            _topic = topic;

            SetValueDeserializer(new MassTransitJsonDeserializer<TValue>());
            SetKeyDeserializer(new MassTransitJsonDeserializer<TKey>());
            SetHeadersDeserializer(headersDeserializer);

            CheckpointInterval = TimeSpan.FromMinutes(1);
            CheckpointMessageCount = 5000;
            ConcurrencyLimit = 1;
        }

        public AutoOffsetReset? AutoOffsetReset
        {
            set => _consumerConfig.AutoOffsetReset = value;
        }

        public string GroupInstanceId
        {
            set => _consumerConfig.GroupInstanceId = value;
        }

        public TimeSpan CheckpointInterval
        {
            set => _checkpointInterval = value;
        }

        public int ConcurrencyLimit
        {
            set => _concurrencyLimit = value;
        }

        public ushort CheckpointMessageCount
        {
            set => _checkpointMessageCount = value;
        }

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

        public IKafkaReceiveEndpoint Build()
        {
            ReceiveEndpointContext CreateContext()
            {
                var builder = new RiderReceiveEndpointBuilder(_busInstance, _endpointConfiguration);

                foreach (var specification in Specifications)
                    specification.Configure(builder);

                return builder.CreateReceiveEndpointContext();
            }

            var context = CreateContext();


            ConsumerBuilder<TKey, TValue> consumerBuilder = CreateConsumerBuilder(context);
            var lockContext = new ConsumerLockContext<TKey, TValue>(consumerBuilder, context.LogContext, _checkpointInterval, _checkpointMessageCount);

            var receiver = new KafkaMessageReceiver<TKey, TValue>(context, lockContext, _headersDeserializer);

            context.AddOrUpdatePayload(() => lockContext, _ => lockContext);

            return new KafkaReceiveEndpoint<TKey, TValue>(_topic, Math.Max(1000, _checkpointMessageCount / 10), _concurrencyLimit, consumerBuilder.Build(),
                receiver, context);
        }

        ConsumerBuilder<TKey, TValue> CreateConsumerBuilder(ReceiveEndpointContext context)
        {
            ConsumerBuilder<TKey, TValue> consumerBuilder = new ConsumerBuilder<TKey, TValue>(_consumerConfig)
                .SetValueDeserializer(_valueDeserializer)
                .SetLogHandler((c, message) => context.LogContext.Info?.Log(message.Message))
                .SetStatisticsHandler((c, value) => context.LogContext.Debug?.Log(value))
                .SetErrorHandler((c, error) => context.LogContext.Error?.Log("Consumer error ({Code}): {Reason} on {Topic}", error.Code, error.Reason, _topic));

            if (_keyDeserializer != null)
                consumerBuilder.SetKeyDeserializer(_keyDeserializer);
            if (_offsetsCommittedHandler != null)
                consumerBuilder.SetOffsetsCommittedHandler(_offsetsCommittedHandler);

            return consumerBuilder;
        }
    }
}
