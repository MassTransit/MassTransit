namespace MassTransit.KafkaIntegration.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using GreenPipes;
    using Serializers;


    public interface ProducerContext<TKey, TValue> :
        PipeContext,
        IDisposable
        where TValue : class
    {
        IHeadersSerializer HeadersSerializer { get; }

        Task Produce(TopicPartition partition, Message<TKey, TValue> message, CancellationToken cancellationToken);
    }
}
