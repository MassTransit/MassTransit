namespace MassTransit.KafkaIntegration
{
    using System.Collections.Generic;
    using Configuration;
    using Confluent.Kafka.Admin;
    using GreenPipes;


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

        public IEnumerable<ValidationResult> Validate()
        {
            if (_numPartitions == 0)
                yield return this.Failure(nameof(NumPartitions), "should be greater than 0");

            if (_replicaFactor <= 0)
                yield return this.Failure(nameof(ReplicationFactor), "should be greater than 0");
        }

        internal TopicSpecification ToSpecification()
        {
            return new TopicSpecification
            {
                Name = _topic,
                NumPartitions = _numPartitions,
                ReplicationFactor = _replicaFactor,
            };
        }
    }
}
