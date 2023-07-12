namespace MassTransit.KafkaIntegration
{
    using System.Collections.Generic;
    using Confluent.Kafka;


    public interface KafkaConsumerBuilderContext
    {
        IEnumerable<TopicPartitionOffset> OnAssigned(IConsumer<byte[], byte[]> consumer, IEnumerable<TopicPartition> partitions);
        IEnumerable<TopicPartitionOffset> OnUnAssigned(IConsumer<byte[], byte[]> consumer, IEnumerable<TopicPartitionOffset> partitions);
        IEnumerable<TopicPartitionOffset> OnPartitionLost(IConsumer<byte[], byte[]> consumer, IEnumerable<TopicPartitionOffset> partitions);
    }
}
