namespace MassTransit.KafkaIntegration.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using Configuration.Configurators;
    using Confluent.Kafka;
    using Context;
    using GreenPipes;
    using MassTransit.Configuration;
    using Registration;
    using Registration.Attachments;
    using Serializers;
    using Transport;


    public class KafkaTopicConfigurator<TKey, TValue> :
        ReceiverConfiguration,
        IKafkaTopicConfigurator<TKey, TValue>
        where TValue : class
    {
        readonly IBusInstance _busInstance;
        readonly ConsumerConfig _consumerConfig;
        readonly IReceiveEndpointConfiguration _endpointConfiguration;
        readonly string _topic;
        IHeadersDeserializer _headersDeserializer;
        IDeserializer<TKey> _keyDeserializer;
        Action<IConsumer<TKey, TValue>, CommittedOffsets> _offsetsCommittedHandler;
        Func<IConsumer<TKey, TValue>, List<TopicPartition>, IEnumerable<TopicPartitionOffset>> _partitionAssignmentHandler;
        Func<IConsumer<TKey, TValue>, List<TopicPartitionOffset>, IEnumerable<TopicPartitionOffset>> _partitionsRevokedHandler;
        IDeserializer<TValue> _valueDeserializer;

        public KafkaTopicConfigurator(ConsumerConfig consumerConfig, string topic, IBusInstance busInstance,
            IReceiveEndpointConfiguration endpointConfiguration, IHeadersDeserializer headersDeserializer)
            : base(endpointConfiguration)
        {
            _busInstance = busInstance;
            _endpointConfiguration = endpointConfiguration;
            _consumerConfig = consumerConfig;
            _topic = topic;
            SetValueDeserializer(new MassTransitSerializer<TValue>());
            SetHeadersDeserializer(headersDeserializer);
        }

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

        public void DisableAutoCommit()
        {
            _consumerConfig.EnableAutoCommit = false;
            _consumerConfig.AutoCommitIntervalMs = null;
        }

        public void EnableAutoCommit(TimeSpan interval)
        {
            _consumerConfig.EnableAutoCommit = true;
            _consumerConfig.AutoCommitIntervalMs = interval.Milliseconds;
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

        public void SetPartitionsAssignedHandler(
            Func<IConsumer<TKey, TValue>, List<TopicPartition>, IEnumerable<TopicPartitionOffset>> partitionsAssignedHandler)
        {
            if (_partitionAssignmentHandler != null)
                throw new InvalidOperationException("Partitions assignment handler may not be specified more than once.");
            _partitionAssignmentHandler = partitionsAssignedHandler ?? throw new ArgumentNullException(nameof(partitionsAssignedHandler));
        }

        public void SetPartitionsAssignedHandler(Action<IConsumer<TKey, TValue>, List<TopicPartition>> partitionAssignmentHandler)
        {
            SetPartitionsAssignedHandler((consumer, partitions) =>
            {
                partitionAssignmentHandler(consumer, partitions);
                return partitions.Select(tp => new TopicPartitionOffset(tp, Offset.Unset)).ToList();
            });
        }

        public void SetPartitionsRevokedHandler(
            Func<IConsumer<TKey, TValue>, List<TopicPartitionOffset>, IEnumerable<TopicPartitionOffset>> partitionsRevokedHandler)
        {
            if (_partitionsRevokedHandler != null)
                throw new InvalidOperationException("Partitions revoked handler may not be specified more than once.");
            _partitionsRevokedHandler = partitionsRevokedHandler ?? throw new ArgumentNullException(nameof(partitionsRevokedHandler));
        }

        public void SetPartitionsRevokedHandler(Action<IConsumer<TKey, TValue>, List<TopicPartitionOffset>> partitionsRevokedHandler)
        {
            SetPartitionsRevokedHandler((consumer, partitions) =>
            {
                partitionsRevokedHandler(consumer, partitions);
                return new List<TopicPartitionOffset>();
            });
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

        public IKafkaConsumer Build()
        {
            IKafkaReceiver<TKey, TValue> CreateReceiver()
            {
                var builder = new BusAttachmentReceiveEndpointBuilder(_busInstance, _endpointConfiguration);

                foreach (var specification in Specifications)
                    specification.Configure(builder);

                return new KafkaReceiver<TKey, TValue>(builder.CreateReceiveEndpointContext(), _headersDeserializer);
            }

            IConsumer<TKey, TValue> CreateConsumer()
            {
                ConsumerBuilder<TKey, TValue> consumerBuilder = new ConsumerBuilder<TKey, TValue>(_consumerConfig)
                    .SetValueDeserializer(_valueDeserializer)
                    .SetLogHandler((c, message) => LogContext.Info?.Log(message.Message))
                    .SetStatisticsHandler((c, value) => LogContext.Debug?.Log(value))
                    .SetErrorHandler((c, error) => LogContext.Error?.Log("Consumer error ({code}): {reason} on {topic}", error.Code, error.Reason, _topic));

                if (_keyDeserializer != null)
                    consumerBuilder.SetKeyDeserializer(_keyDeserializer);

                if (_offsetsCommittedHandler != null)
                    consumerBuilder.SetOffsetsCommittedHandler(_offsetsCommittedHandler);

                if (_partitionsRevokedHandler != null)
                    consumerBuilder.SetPartitionsRevokedHandler(_partitionsRevokedHandler);
                if (_partitionAssignmentHandler != null)
                    consumerBuilder.SetPartitionsAssignedHandler(_partitionAssignmentHandler);

                return consumerBuilder.Build();
            }

            LogContext.SetCurrentIfNull(_busInstance.HostConfiguration.LogContext);

            return new KafkaConsumer<TKey, TValue>(_topic, CreateConsumer(), CreateReceiver(), _busInstance.HostConfiguration.LogContext,
                _consumerConfig.EnableAutoCommit == true);
        }
    }
}
