namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Checkpoints;
    using Confluent.Kafka;
    using Util;


    public class ConsumerLockContext :
        IConsumerLockContext,
        KafkaConsumerBuilderContext,
        IChannelExecutorPool<ConsumeResult<byte[], byte[]>>
    {
        readonly ConsumerContext _context;
        readonly SingleThreadedDictionary<TopicPartition, PartitionCheckpointData> _data;
        readonly PendingConfirmationCollection _pending;
        readonly ReceiveSettings _receiveSettings;

        public ConsumerLockContext(ConsumerContext context, ReceiveSettings receiveSettings, CancellationToken cancellationToken)
        {
            _context = context;
            _receiveSettings = receiveSettings;
            _pending = new PendingConfirmationCollection(cancellationToken);
            _data = new SingleThreadedDictionary<TopicPartition, PartitionCheckpointData>();
        }

        public Task Push(ConsumeResult<byte[], byte[]> partition, Func<Task> method, CancellationToken cancellationToken = default)
        {
            return _data.TryGetValue(partition.TopicPartition, out var data) ? data.Push(method) : Task.CompletedTask;
        }

        public Task Run(ConsumeResult<byte[], byte[]> partition, Func<Task> method, CancellationToken cancellationToken = default)
        {
            return _data.TryGetValue(partition.TopicPartition, out var data) ? data.Run(method) : Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            _pending.Dispose();
            return default;
        }

        public Task Pending(ConsumeResult<byte[], byte[]> result)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            return _data.TryGetValue(result.TopicPartition, out var data) ? data.Pending(result) : Task.CompletedTask;
        }

        public Task Complete(ConsumeResult<byte[], byte[]> result)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            _pending.Complete(result.TopicPartitionOffset);

            return Task.CompletedTask;
        }

        public void Canceled(ConsumeResult<byte[], byte[]> result, CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            _pending.Canceled(result.TopicPartitionOffset, cancellationToken);
        }

        public Task Faulted(ConsumeResult<byte[], byte[]> result, Exception exception)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            _pending.Faulted(result.TopicPartitionOffset, exception);

            return Task.CompletedTask;
        }

        public IEnumerable<TopicPartitionOffset> OnAssigned(IConsumer<byte[], byte[]> consumer, IEnumerable<TopicPartition> partitions)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            foreach (var partition in partitions)
            {
                if (!_data.TryAdd(partition, p => new PartitionCheckpointData(consumer, _receiveSettings, _pending)))
                    continue;

                LogContext.Debug?.Log("Partition: {PartitionId} with {Offset} was assigned to: {MemberId}", partition, _receiveSettings.Offset,
                    consumer.MemberId);

                yield return new TopicPartitionOffset(partition, _receiveSettings.Offset);
            }
        }

        public IEnumerable<TopicPartitionOffset> OnPartitionLost(IConsumer<byte[], byte[]> consumer, IEnumerable<TopicPartitionOffset> partitions)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            Task LostAndDelete(TopicPartitionOffset topicPartition)
            {
                if (!_data.TryRemove(topicPartition.TopicPartition, out var data))
                    return Task.CompletedTask;

                LogContext.Debug?.Log("Partition: {PartitionId} was lost on {MemberId}", topicPartition.TopicPartition, consumer.MemberId);
                return data.Lost();
            }

            TaskUtil.Await(Task.WhenAll(partitions.Select(partition => LostAndDelete(partition))));
            return Array.Empty<TopicPartitionOffset>();
        }

        public IEnumerable<TopicPartitionOffset> OnUnAssigned(IConsumer<byte[], byte[]> consumer, IEnumerable<TopicPartitionOffset> partitions)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            Task CloseAndDelete(TopicPartitionOffset topicPartition)
            {
                if (!_data.TryRemove(topicPartition.TopicPartition, out var data))
                    return Task.CompletedTask;

                LogContext.Debug?.Log("Partition: {PartitionId} was closed on {MemberId}", topicPartition.TopicPartition, consumer.MemberId);
                return data.Close();
            }

            TaskUtil.Await(Task.WhenAll(partitions.Select(partition => CloseAndDelete(partition))));
            return Array.Empty<TopicPartitionOffset>();
        }


        sealed class PartitionCheckpointData
        {
            readonly CancellationTokenSource _cancellationTokenSource;
            readonly ICheckpointer _checkpointer;
            readonly ChannelExecutor _executor;
            readonly PendingConfirmationCollection _pending;

            public PartitionCheckpointData(IConsumer<byte[], byte[]> consumer, ReceiveSettings settings, PendingConfirmationCollection pending)
            {
                _pending = pending;
                _cancellationTokenSource = new CancellationTokenSource();

                _executor = new ChannelExecutor(settings.PrefetchCount, settings.ConcurrentMessageLimit);
                _checkpointer = new BatchCheckpointer(consumer, settings, _cancellationTokenSource.Token);
            }

            public Task Pending(ConsumeResult<byte[], byte[]> result)
            {
                var pendingConfirmation = _pending.Add(result.TopicPartitionOffset);
                return _checkpointer.Pending(pendingConfirmation);
            }

            public Task Lost()
            {
                _cancellationTokenSource.Cancel();
                return Close();
            }

            public async Task Close()
            {
                await _executor.DisposeAsync().ConfigureAwait(false);
                await _checkpointer.DisposeAsync().ConfigureAwait(false);

                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }

            public Task Push(Func<Task> method)
            {
                return _executor.Push(method, _cancellationTokenSource.Token);
            }

            public Task Run(Func<Task> method)
            {
                return _executor.Run(method, _cancellationTokenSource.Token);
            }
        }
    }
}
