namespace MassTransit.KafkaIntegration.Contexts
{
    using System.Threading.Tasks;
    using Confluent.Kafka;


    public interface IConsumerLockContext<TKey, TValue>
        where TValue : class
    {
        Task Complete(ConsumeResult<TKey, TValue> result);
    }
}
