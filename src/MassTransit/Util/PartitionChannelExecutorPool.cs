namespace MassTransit.Util
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Middleware;


    public class PartitionChannelExecutorPool<T> :
        IChannelExecutorPool<T>
    {
        readonly IHashGenerator _hashGenerator;
        readonly PartitionKeyProvider<T> _partitionKeyProvider;
        readonly Lazy<TaskExecutor>[] _partitions;

        public PartitionChannelExecutorPool(PartitionKeyProvider<T> partitionKeyProvider, IHashGenerator hashGenerator, int concurrencyLimit,
            int concurrentDeliveryLimit = 1)
        {
            _partitionKeyProvider = partitionKeyProvider;
            _hashGenerator = hashGenerator;
            _partitions = Enumerable.Range(0, concurrencyLimit)
                .Select(_ => new Lazy<TaskExecutor>(() => new TaskExecutor(concurrentDeliveryLimit)))
                .ToArray();
        }

        public async ValueTask DisposeAsync()
        {
            foreach (Lazy<TaskExecutor> partition in _partitions)
            {
                if (partition.IsValueCreated)
                    await partition.Value.DisposeAsync().ConfigureAwait(false);
            }
        }

        public Task Push(T partition, Func<Task> method, CancellationToken cancellationToken = default)
        {
            var executor = GetChannelExecutor(partition);
            return executor.Push(method, cancellationToken);
        }

        public Task Run(T partition, Func<Task> method, CancellationToken cancellationToken = default)
        {
            var executor = GetChannelExecutor(partition);
            return executor.Run(method, cancellationToken);
        }

        TaskExecutor GetChannelExecutor(T partition)
        {
            if (_partitions.Length == 1)
                return _partitions[0].Value;

            var partitionKey = _partitionKeyProvider(partition);
            var hash = partitionKey?.Length > 0 ? _hashGenerator.Hash(partitionKey) : 0;
            var index = hash % _partitions.Length;
            return _partitions[index].Value;
        }
    }
}
