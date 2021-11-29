namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;


    public interface IConsumerLockContext<TKey, TValue>
        where TValue : class
    {
        Task Pending(ConsumeResult<TKey, TValue> result);
        Task Complete(ConsumeResult<TKey, TValue> result);
        Task Faulted(ConsumeResult<TKey, TValue> result, Exception exception);
    }
}
