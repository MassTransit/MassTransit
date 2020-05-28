namespace MassTransit.KafkaIntegration.Contexts
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using GreenPipes.Util;
    using Transports;


    public class ConsumeResultLockContext<TKey, TValue> :
        ReceiveLockContext
        where TValue : class
    {
        readonly ConsumeResult<TKey, TValue> _consumeResult;
        readonly IConsumerLockContext<TKey, TValue> _lockContext;

        public ConsumeResultLockContext(IConsumerLockContext<TKey, TValue> lockContext, ConsumeResult<TKey, TValue> consumeResult)
        {
            _lockContext = lockContext;
            _consumeResult = consumeResult;
        }

        public Task Complete()
        {
            return _lockContext.Complete(_consumeResult);
        }

        public Task Faulted(Exception exception)
        {
            return TaskUtil.Completed;
        }

        public Task ValidateLockStatus()
        {
            return TaskUtil.Completed;
        }
    }
}
