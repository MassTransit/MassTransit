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
        KafkaConsumerBuilderContext
    {
        readonly SingleThreadedDictionary<TopicPartition, PartitionCheckpointData> _data =
            new SingleThreadedDictionary<TopicPartition, PartitionCheckpointData>();

        readonly ConsumerContext _context;
        readonly ReceiveSettings _receiveSettings;
        readonly CancellationToken _cancellationToken;

        public ConsumerLockContext(ConsumerContext context, ReceiveSettings receiveSettings, CancellationToken cancellationToken)
        {
            _context = context;
            _receiveSettings = receiveSettings;
            _cancellationToken = cancellationToken;
        }

        public async Task Pending(ConsumeResult<byte[], byte[]> result)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            if (_data.TryGetValue(result.TopicPartition, out var data))
                await data.Pending(result).ConfigureAwait(false);
        }

        public Task Complete(ConsumeResult<byte[], byte[]> result)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            if (_data.TryGetValue(result.TopicPartition, out var data))
                data.Complete(result);

            return Task.CompletedTask;
        }

        public Task Faulted(ConsumeResult<byte[], byte[]> result, Exception exception)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            if (_data.TryGetValue(result.TopicPartition, out var data))
                data.Faulted(result, exception);

            return Task.CompletedTask;
        }

        public void OnAssigned(IConsumer<byte[], byte[]> consumer, IEnumerable<TopicPartition> partitions)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            foreach (var partition in partitions)
            {
                if (!_data.TryAdd(partition, p => new PartitionCheckpointData(partition, consumer, _receiveSettings, _cancellationToken)))
                    continue;

                LogContext.Debug?.Log("Partition: {PartitionId} was assigned to: {MemberId}", partition, consumer.MemberId);
            }
        }

        public void OnPartitionLost(IConsumer<byte[], byte[]> consumer, IEnumerable<TopicPartitionOffset> partitions)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            async Task<bool> LostAndDelete(TopicPartitionOffset topicPartition)
            {
                if (!_data.TryGetValue(topicPartition.TopicPartition, out var data))
                    return false;

                await data.Lost().ConfigureAwait(false);
                LogContext.Debug?.Log("Partition: {PartitionId} was lost on {MemberId}", topicPartition.TopicPartition, consumer.MemberId);
                return _data.TryRemove(topicPartition.TopicPartition, out _);
            }

            TaskUtil.Await(Task.WhenAll(partitions.Select(partition => LostAndDelete(partition))));
        }

        public void OnUnAssigned(IConsumer<byte[], byte[]> consumer, IEnumerable<TopicPartitionOffset> partitions)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            async Task<bool> CloseAndDelete(TopicPartitionOffset topicPartition)
            {
                if (!_data.TryGetValue(topicPartition.TopicPartition, out var data))
                    return false;

                await data.Close().ConfigureAwait(false);
                LogContext.Debug?.Log("Partition: {PartitionId} was closed on {MemberId}", topicPartition.TopicPartition, consumer.MemberId);
                return _data.TryRemove(topicPartition.TopicPartition, out _);
            }

            TaskUtil.Await(Task.WhenAll(partitions.Select(partition => CloseAndDelete(partition))));
        }


        sealed class PartitionCheckpointData
        {
            readonly CancellationToken _cancellationToken;
            readonly ICheckpointer _checkpointer;
            readonly ChannelExecutor _executor;
            readonly PendingConfirmationCollection _pending;
            readonly CancellationTokenSource _cancellationTokenSource;

            public PartitionCheckpointData(TopicPartition partition, IConsumer<byte[], byte[]> consumer, ReceiveSettings settings,
                CancellationToken cancellationToken)
            {
                _cancellationToken = cancellationToken;
                _cancellationTokenSource = new CancellationTokenSource();
                _pending = new PendingConfirmationCollection(partition, _cancellationToken);
                _executor = new ChannelExecutor(settings.PrefetchCount, settings.ConcurrentMessageLimit);
                _checkpointer = new BatchCheckpointer(consumer, settings, _cancellationTokenSource.Token);
            }

            public Task Pending(ConsumeResult<byte[], byte[]> result)
            {
                if (_cancellationToken.IsCancellationRequested)
                    return Task.CompletedTask;

                var pendingConfirmation = _pending.Add(result.Offset);
                return _checkpointer.Pending(pendingConfirmation);
            }

            public void Complete(ConsumeResult<byte[], byte[]> result)
            {
                _pending.Complete(result.Offset);
            }

            public void Faulted(ConsumeResult<byte[], byte[]> result, Exception exception)
            {
                _pending.Faulted(result.Offset, exception);
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

                _pending.Dispose();
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


        public async ValueTask DisposeAsync()
        {
        }

        public Task Push(ConsumeResult<byte[], byte[]> partition, Func<Task> method, CancellationToken cancellationToken = default)
        {
            return _data[partition.TopicPartition].Push(method);
        }

        public Task Run(ConsumeResult<byte[], byte[]> partition, Func<Task> method, CancellationToken cancellationToken = default)
        {
            return _data[partition.TopicPartition].Run(method);
        }
    }
}
