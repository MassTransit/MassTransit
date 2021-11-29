namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Serializers;


    public interface ConsumerContext<TKey, TValue> :
        PipeContext,
        IConsumerLockContext<TKey, TValue>,
        IAsyncDisposable
        where TValue : class
    {
        ReceiveSettings ReceiveSettings { get; }
        IHeadersDeserializer HeadersDeserializer { get; }
        event Action<IConsumer<TKey, TValue>, Error> ErrorHandler;

        Task Subscribe();
        Task Close();

        Task<ConsumeResult<TKey, TValue>> Consume(CancellationToken cancellationToken);
    }
}
