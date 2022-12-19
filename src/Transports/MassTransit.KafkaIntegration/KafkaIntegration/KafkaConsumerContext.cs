namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using MassTransit.Configuration;
    using MassTransit.Middleware;


    public class KafkaConsumerContext :
        BasePipeContext,
        ConsumerContext
    {
        readonly ConsumerBuilderFactory _consumerBuilderFactory;
        readonly IConsumerLockContext _lockContext;

        public KafkaConsumerContext(IHostConfiguration hostConfiguration, ReceiveSettings receiveSettings,
            ConsumerBuilderFactory consumerBuilderFactory, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            var lockContext = new ConsumerLockContext(hostConfiguration, receiveSettings);
            _consumerBuilderFactory = () => consumerBuilderFactory()
                .SetPartitionsAssignedHandler(lockContext.OnAssigned)
                .SetPartitionsRevokedHandler(lockContext.OnUnAssigned);
            _lockContext = lockContext;
        }

        public event Action<Error> ErrorHandler;

        public IConsumer<byte[], byte[]> CreateConsumer(Action<IConsumer<byte[], byte[]>, Error> onError)
        {
            return _consumerBuilderFactory()
                .SetErrorHandler((consumer, error) =>
                {
                    onError?.Invoke(consumer, error);
                    ErrorHandler?.Invoke(error);
                })
                .Build();
        }

        public Task Pending(ConsumeResult<byte[], byte[]> result)
        {
            return _lockContext.Pending(result);
        }

        public Task Complete(ConsumeResult<byte[], byte[]> result)
        {
            return _lockContext.Complete(result);
        }

        public Task Faulted(ConsumeResult<byte[], byte[]> result, Exception exception)
        {
            return _lockContext.Faulted(result, exception);
        }

        public Task Push(ConsumeResult<byte[], byte[]> result, Func<Task> method, CancellationToken cancellationToken = default)
        {
            return _lockContext.Push(result, method, cancellationToken);
        }

        public Task Run(ConsumeResult<byte[], byte[]> result, Func<Task> method, CancellationToken cancellationToken = default)
        {
            return _lockContext.Push(result, method, cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            return _lockContext.DisposeAsync();
        }
    }
}
