namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using Configuration.Configurators;
    using Confluent.Kafka;
    using Context;


    public class KafkaSubscriptionConfigurator<TKey, TValue> :
        IKafkaSubscriptionConfigurator<TKey, TValue>
        where TValue : class
    {
        readonly ConsumerConfig _consumerConfig;
        Action<IConsumer<TKey, TValue>, Error> _errorHandler;
        IDeserializer<TKey> _keyDeserializer;
        Action<IConsumer<TKey, TValue>, LogMessage> _logHandler;
        Action<IConsumer<TKey, TValue>, CommittedOffsets> _offsetsCommittedHandler;
        Func<IConsumer<TKey, TValue>, List<TopicPartition>, IEnumerable<TopicPartitionOffset>> _partitionAssignmentHandler;
        Func<IConsumer<TKey, TValue>, List<TopicPartitionOffset>, IEnumerable<TopicPartitionOffset>> _partitionsRevokedHandler;
        Action<IConsumer<TKey, TValue>, string> _statisticsHandler;
        IDeserializer<TValue> _valueDeserializer;

        public KafkaSubscriptionConfigurator(ClientConfig clientConfig)
        {
            _consumerConfig = new ConsumerConfig(clientConfig);
            EnableAutoCommit(TimeSpan.FromSeconds(5));
        }

        public AutoOffsetReset? AutoOffsetReset
        {
            set => _consumerConfig.AutoOffsetReset = value;
        }

        public string GroupId
        {
            set => _consumerConfig.GroupId = value;
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
            if (_keyDeserializer != null)
                throw new InvalidOperationException("Key deserializer may not be specified more than once.");
            _keyDeserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        }

        public void SetValueDeserializer(IDeserializer<TValue> deserializer)
        {
            if (_valueDeserializer != null)
                throw new InvalidOperationException("Value deserializer may not be specified more than once.");
            _valueDeserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
        }

        public void SetErrorHandler(Action<IConsumer<TKey, TValue>, Error> errorHandler)
        {
            if (_errorHandler != null)
                throw new InvalidOperationException("Error handler may not be specified more than once.");
            _errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
        }

        public void SetLogHandler(Action<IConsumer<TKey, TValue>, LogMessage> logHandler)
        {
            if (_logHandler != null)
                throw new InvalidOperationException("Log handler may not be specified more than once.");
            _logHandler = logHandler ?? throw new ArgumentNullException(nameof(logHandler));
        }

        public void SetStatisticsHandler(Action<IConsumer<TKey, TValue>, string> statisticsHandler)
        {
            if (_statisticsHandler != null)
                throw new InvalidOperationException("Statistics handler may not be specified more than once.");

            _statisticsHandler = statisticsHandler ?? throw new ArgumentNullException(nameof(statisticsHandler));
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

        public KafkaSubscription<TKey, TValue> Build(ITopicNameFormatter entityNameFormatter, ILogContext logContext)
        {
            var topic = entityNameFormatter.GetTopicName<TKey, TValue>();

            ConsumerBuilder<TKey, TValue> consumerBuilder = new ConsumerBuilder<TKey, TValue>(_consumerConfig)
                .SetLogHandler((consumer, message) =>
                {
                    logContext.Info?.Log(message.Message);
                    _logHandler?.Invoke(consumer, message);
                })
                .SetStatisticsHandler((consumer, value) =>
                {
                    logContext.Debug?.Log(value);
                    _statisticsHandler?.Invoke(consumer, value);
                })
                .SetErrorHandler((consumer, error) =>
                {
                    logContext.Error?.Log("Consumer error ({code}): {reason} on {topic}", error.Code, error.Reason, topic);
                    _errorHandler?.Invoke(consumer, error);
                });


            if (_keyDeserializer != null)
                consumerBuilder.SetKeyDeserializer(_keyDeserializer);

            if (_valueDeserializer != null)
                consumerBuilder.SetValueDeserializer(_valueDeserializer);

            if (_offsetsCommittedHandler != null)
                consumerBuilder.SetOffsetsCommittedHandler(_offsetsCommittedHandler);

            if (_partitionsRevokedHandler != null)
                consumerBuilder.SetPartitionsRevokedHandler(_partitionsRevokedHandler);
            if (_partitionAssignmentHandler != null)
                consumerBuilder.SetPartitionsAssignedHandler(_partitionAssignmentHandler);

            return new KafkaSubscription<TKey, TValue>(topic, consumerBuilder.Build(), logContext, _consumerConfig.EnableAutoCommit == true);
        }
    }
}
