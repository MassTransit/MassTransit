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
            return [];
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
            return [];
        }
    }
}
