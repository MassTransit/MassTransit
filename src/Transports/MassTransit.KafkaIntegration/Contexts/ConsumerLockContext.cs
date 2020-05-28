namespace MassTransit.KafkaIntegration.Contexts
{
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Util;


    public interface IConsumerLockContext<TKey, TValue>
        where TValue : class
    {
        Task Complete(ConsumeResult<TKey, TValue> result);
    }


    public class ConsumerLockContext<TKey, TValue> :
        IConsumerLockContext<TKey, TValue>
        where TValue : class
    {
        readonly IConsumer<TKey, TValue> _consumer;
        readonly bool _isAutoCommitDisabled;

        public ConsumerLockContext(IConsumer<TKey, TValue> consumer, ConsumerConfig consumerConfig)
        {
            _consumer = consumer;
            _isAutoCommitDisabled = consumerConfig.EnableAutoCommit == false;
        }

        public Task Complete(ConsumeResult<TKey, TValue> result)
        {
            if (_isAutoCommitDisabled)
                _consumer.Commit(result);
            return TaskUtil.Completed;
        }
    }
}
