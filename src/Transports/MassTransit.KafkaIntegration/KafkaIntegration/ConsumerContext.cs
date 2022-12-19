namespace MassTransit.KafkaIntegration
{
    using System;
    using Confluent.Kafka;
    using Util;


    public interface ConsumerContext :
        PipeContext,
        IConsumerLockContext,
        IChannelExecutorPool<ConsumeResult<byte[], byte[]>>
    {
        event Action<Error> ErrorHandler;
        IConsumer<byte[], byte[]> CreateConsumer(Action<IConsumer<byte[], byte[]>, Error> onError);
    }
}
