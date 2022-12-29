namespace MassTransit.KafkaIntegration
{
    using System;
    using Confluent.Kafka;


    public interface ConsumerContext :
        PipeContext,
        IConsumerLockContext
    {
        event Action<Error> ErrorHandler;
        IConsumer<byte[], byte[]> CreateConsumer(Action<IConsumer<byte[], byte[]>, Error> onError);
    }
}
