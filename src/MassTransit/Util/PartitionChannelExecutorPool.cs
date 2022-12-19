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
        readonly PartitionKeyProvider<T> _partitionKeyProvider;
        readonly IHashGenerator _hashGenerator;
        readonly ChannelExecutor[] _partitions;

        public PartitionChannelExecutorPool(PartitionKeyProvider<T> partitionKeyProvider, IHashGenerator hashGenerator, int concurrencyLimit,
            int concurrentDeliveryLimit = 1)
        {
            _partitionKeyProvider = partitionKeyProvider;
            _hashGenerator = hashGenerator;
            _partitions = Enumerable.Range(0, concurrencyLimit)
                .Select(x => new ChannelExecutor(concurrentDeliveryLimit))
                .ToArray();
        }

        public Task Push(T partition, Func<Task> handle, CancellationToken cancellationToken)
        {
            var executor = GetExecutor(partition);
            return executor.Push(handle, cancellationToken);
        }

        public Task Run(T partition, Func<Task> method, CancellationToken cancellationToken = default)
        {
            var executor = GetExecutor(partition);
            return executor.Run(method, cancellationToken);
        }

        ChannelExecutor GetExecutor(T partition)
        {
            var partitionKey = _partitionKeyProvider(partition);
            var hash = partitionKey?.Length > 0 ? _hashGenerator.Hash(partitionKey) : 0;
            var index = hash % _partitions.Length;
            return _partitions[index];
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var partition in _partitions)
                await partition.DisposeAsync().ConfigureAwait(false);
        }
    }
}
