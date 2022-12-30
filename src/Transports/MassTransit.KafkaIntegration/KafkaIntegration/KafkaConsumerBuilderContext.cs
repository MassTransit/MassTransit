namespace MassTransit.KafkaIntegration
{
    using System.Collections.Generic;
    using Confluent.Kafka;


    public interface KafkaConsumerBuilderContext
    {
        void OnAssigned(IConsumer<byte[], byte[]> consumer, IEnumerable<TopicPartition> partitions);
        void OnUnAssigned(IConsumer<byte[], byte[]> consumer, IEnumerable<TopicPartitionOffset> partitions);
        void OnPartitionLost(IConsumer<byte[], byte[]> consumer, IEnumerable<TopicPartitionOffset> partitions);
    }
}
