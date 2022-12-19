namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Util;


    public interface IConsumerLockContext :
        IChannelExecutorPool<ConsumeResult<byte[], byte[]>>
    {
        Task Pending(ConsumeResult<byte[], byte[]> result);
        Task Complete(ConsumeResult<byte[], byte[]> result);
        Task Faulted(ConsumeResult<byte[], byte[]> result, Exception exception);
    }
}
