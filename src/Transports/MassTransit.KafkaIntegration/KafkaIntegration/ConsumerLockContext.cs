namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Checkpoints;
    using Confluent.Kafka;
    using MassTransit.Configuration;
    using Util;


    public class ConsumerLockContext :
        IConsumerLockContext
    {
        readonly SingleThreadedDictionary<TopicPartition, PartitionCheckpointData> _data =
            new SingleThreadedDictionary<TopicPartition, PartitionCheckpointData>();

        readonly IHostConfiguration _hostConfiguration;
        readonly ReceiveSettings _receiveSettings;

        public ConsumerLockContext(IHostConfiguration hostConfiguration, ReceiveSettings receiveSettings)
        {
            _hostConfiguration = hostConfiguration;
            _receiveSettings = receiveSettings;
        }

        public async Task Pending(ConsumeResult<byte[], byte[]> result)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            if (_data.TryGetValue(result.TopicPartition, out var data))
                await data.Pending(result).ConfigureAwait(false);
        }

        public Task Complete(ConsumeResult<byte[], byte[]> result)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            if (_data.TryGetValue(result.TopicPartition, out var data))
                data.Complete(result);

            return Task.CompletedTask;
        }

        public Task Faulted(ConsumeResult<byte[], byte[]> result, Exception exception)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            if (_data.TryGetValue(result.TopicPartition, out var data))
                data.Faulted(result, exception);

            return Task.CompletedTask;
        }

        public void OnAssigned(IConsumer<byte[], byte[]> consumer, IEnumerable<TopicPartition> partitions)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            foreach (var partition in partitions)
            {
                if (!_data.TryAdd(partition, p => new PartitionCheckpointData(partition, consumer, _receiveSettings)))
                    continue;

                LogContext.Debug?.Log("Partition: {PartitionId} was assigned to: {MemberId}", partition, consumer.MemberId);
            }
        }

        public void OnUnAssigned(IConsumer<byte[], byte[]> consumer, IEnumerable<TopicPartitionOffset> partitions)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            async Task<bool> CloseAndDelete(TopicPartitionOffset topicPartition)
            {
                if (!_data.TryGetValue(topicPartition.TopicPartition, out var data))
                    return false;

                await data.Close(topicPartition).ConfigureAwait(false);
                return _data.TryRemove(topicPartition.TopicPartition, out _);
            }

            TaskUtil.Await(Task.WhenAll(partitions.Select(partition => CloseAndDelete(partition))));
        }


        sealed class PartitionCheckpointData
        {
            readonly ICheckpointer _checkpointer;
            readonly ChannelExecutor _executor;
            readonly PendingConfirmationCollection _pending;
            readonly string _memberId;

            public PartitionCheckpointData(TopicPartition partition, IConsumer<byte[], byte[]> consumer, ReceiveSettings settings)
            {
                _memberId = consumer.MemberId;
                _pending = new PendingConfirmationCollection(partition);
                _executor = new ChannelExecutor(settings.PrefetchCount, settings.ConcurrentMessageLimit);
                _checkpointer = new BatchCheckpointer(consumer, settings);
            }

            public Task Pending(ConsumeResult<byte[], byte[]> result)
            {
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

            public async Task Close(TopicPartitionOffset partition)
            {
                await _executor.DisposeAsync().ConfigureAwait(false);
                await _checkpointer.DisposeAsync().ConfigureAwait(false);

                LogContext.Debug?.Log("Partition: {PartitionId} was closed on {MemberId}", partition.TopicPartition, _memberId);
            }

            public Task Push(Func<Task> method, CancellationToken cancellationToken = default)
            {
                return _executor.Push(method, cancellationToken);
            }

            public Task Run(Func<Task> method, CancellationToken cancellationToken = default)
            {
                return _executor.Run(method, cancellationToken);
            }
        }


        public async ValueTask DisposeAsync()
        {
        }

        public Task Push(ConsumeResult<byte[], byte[]> partition, Func<Task> method, CancellationToken cancellationToken = default)
        {
            return _data[partition.TopicPartition].Push(method, cancellationToken);
        }

        public Task Run(ConsumeResult<byte[], byte[]> partition, Func<Task> method, CancellationToken cancellationToken = default)
        {
            return _data[partition.TopicPartition].Run(method, cancellationToken);
        }
    }
}
