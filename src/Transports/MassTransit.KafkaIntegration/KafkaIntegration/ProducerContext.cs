namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;


    public interface ProducerContext :
        PipeContext,
        IDisposable
    {
        Task Produce(TopicPartition partition, Message<byte[], byte[]> message, CancellationToken cancellationToken);
    }
}
