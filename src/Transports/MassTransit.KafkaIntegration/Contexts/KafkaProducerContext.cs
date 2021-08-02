namespace MassTransit.KafkaIntegration.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using GreenPipes;
    using Serializers;
    using Transport;


    public class KafkaProducerContext<TKey, TValue> :
        BasePipeContext,
        ProducerContext<TKey, TValue>
        where TValue : class
    {
        readonly IProducer<TKey, TValue> _producer;

        public KafkaProducerContext(ProducerBuilder<TKey, TValue> producerBuilder, IHeadersSerializer headersSerializer, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _producer = producerBuilder.Build();
            HeadersSerializer = headersSerializer;
        }

        public IHeadersSerializer HeadersSerializer { get; }

        public Task Produce(TopicPartition partition, Message<TKey, TValue> message, CancellationToken cancellationToken)
        {
            return _producer.ProduceAsync(partition, message, cancellationToken);
        }

        public void Dispose()
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            _producer.Flush(cts.Token);

            _producer.Dispose();
        }
    }
}
