namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using MassTransit.Middleware;


    public class SharedConsumerContext :
        ProxyPipeContext,
        ConsumerContext
    {
        readonly ConsumerContext _context;

        public SharedConsumerContext(ConsumerContext context, CancellationToken cancellationToken)
            : base(context)
        {
            _context = context;
            CancellationToken = cancellationToken;
        }

        public override CancellationToken CancellationToken { get; }

        public event Action<Error> ErrorHandler
        {
            add => _context.ErrorHandler += value;
            remove => _context.ErrorHandler -= value;
        }

        public IConsumer<byte[], byte[]> CreateConsumer(Action<IConsumer<byte[], byte[]>, Error> onError)
        {
            return _context.CreateConsumer(onError);
        }

        public Task Pending(ConsumeResult<byte[], byte[]> result)
        {
            return _context.Pending(result);
        }

        public Task Complete(ConsumeResult<byte[], byte[]> result)
        {
            return _context.Complete(result);
        }

        public Task Faulted(ConsumeResult<byte[], byte[]> result, Exception exception)
        {
            return _context.Faulted(result, exception);
        }

        public Task Push(ConsumeResult<byte[], byte[]> result, Func<Task> method, CancellationToken cancellationToken = default)
        {
            return _context.Push(result, method, cancellationToken);
        }

        public Task Run(ConsumeResult<byte[], byte[]> result, Func<Task> method, CancellationToken cancellationToken = default)
        {
            return _context.Run(result, method, cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            return _context.DisposeAsync();
        }
    }
}
