namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Transports;


    public class KafkaReceiveLockContext :
        ReceiveLockContext
    {
        readonly IConsumerLockContext _lockContext;
        readonly ConsumeResult<byte[], byte[]> _result;

        public KafkaReceiveLockContext(ConsumeResult<byte[], byte[]> result, IConsumerLockContext lockContext)
        {
            _result = result;
            _lockContext = lockContext;
        }

        public Task Complete()
        {
            return _lockContext.Complete(_result);
        }

        public Task Faulted(Exception exception)
        {
            return _lockContext.Faulted(_result, exception);
        }

        public Task ValidateLockStatus()
        {
            return Task.CompletedTask;
        }
    }
}
