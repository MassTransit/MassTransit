namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka.Admin;
    using MassTransit.Configuration;


    public class KafkaTopicOptions :
        IOptions,
        ISpecification
    {
        readonly string _topic;
        ushort _numPartitions;
        short _replicaFactor;

        public KafkaTopicOptions(string topic)
        {
            _topic = topic;
            _numPartitions = 1;
            _replicaFactor = 1;
            RequestTimeout = TimeSpan.FromSeconds(5);
        }

        /// <summary>
        /// The number of partitions for the new topic
        /// </summary>
        public ushort NumPartitions
        {
            set => _numPartitions = value;
        }

        /// <summary>
        /// The replication factor for the new topic (should not be bigger than number of brokers)
        /// </summary>
        public short ReplicationFactor
        {
            set => _replicaFactor = value;
        }

        public TimeSpan RequestTimeout { set; internal get; }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_numPartitions == 0)
                yield return this.Failure(nameof(NumPartitions), "should be greater than 0");

            if (_replicaFactor <= 0)
                yield return this.Failure(nameof(ReplicationFactor), "should be greater than 0");

            if (RequestTimeout <= TimeSpan.Zero)
                yield return this.Failure(nameof(RequestTimeout), "should be greater than 0");
        }

        internal TopicSpecification ToSpecification() =>
            new TopicSpecification
            {
                Name = _topic,
                NumPartitions = _numPartitions,
                ReplicationFactor = _replicaFactor,
            };
    }
}
