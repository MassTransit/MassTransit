namespace MassTransit.KafkaIntegration.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Context;
    using Util;


    public class ConsumerLockContext<TKey, TValue> :
        IConsumerLockContext<TKey, TValue>
        where TValue : class
    {
        readonly SingleThreadedDictionary<Partition, PartitionCheckpointData> _data = new SingleThreadedDictionary<Partition, PartitionCheckpointData>();
        readonly ILogContext _logContext;
        readonly ushort _maxCount;
        readonly TimeSpan _timeout;

        public ConsumerLockContext(ConsumerBuilder<TKey, TValue> builder, ILogContext logContext, TimeSpan timeout, ushort maxCount)
        {
            _logContext = logContext;
            _timeout = timeout;
            _maxCount = maxCount;

            builder.SetPartitionsAssignedHandler(OnAssigned);
            builder.SetPartitionsRevokedHandler(OnUnAssigned);
        }

        public Task Complete(ConsumeResult<TKey, TValue> result)
        {
            LogContext.SetCurrentIfNull(_logContext);

            if (_data.TryGetValue(result.Partition, out var data))
                data.TryCheckpoint(result);

            return TaskUtil.Completed;
        }

        void OnAssigned(IConsumer<TKey, TValue> consumer, IEnumerable<TopicPartition> partitions)
        {
            LogContext.SetCurrentIfNull(_logContext);

            foreach (var partition in partitions)
            {
                if (_data.TryAdd(partition.Partition, p => new PartitionCheckpointData(consumer, _timeout, _maxCount)))
                    LogContext.Info?.Log("Partition: {PartitionId} was assigned", partition);
            }
        }

        void OnUnAssigned(IConsumer<TKey, TValue> consumer, IEnumerable<TopicPartitionOffset> partitions)
        {
            LogContext.SetCurrentIfNull(_logContext);

            foreach (var partition in partitions)
            {
                if (_data.TryRemove(partition.Partition, out var data))
                    data.Close(partition);
            }
        }


        sealed class PartitionCheckpointData
        {
            readonly IConsumer<TKey, TValue> _consumer;
            readonly ushort _maxCount;
            readonly TimeSpan _timeout;
            readonly Stopwatch _timer;
            ConsumeResult<TKey, TValue> _current;
            ushort _processed;

            public PartitionCheckpointData(IConsumer<TKey, TValue> consumer, TimeSpan timeout, ushort maxCount)
            {
                _consumer = consumer;
                _timeout = timeout;
                _maxCount = maxCount;
                _processed = 0;
                _timer = Stopwatch.StartNew();
            }

            public bool TryCheckpoint(ConsumeResult<TKey, TValue> result)
            {
                void Reset()
                {
                    _current = default;
                    _processed = 0;
                    _timer.Restart();
                }

                _current = result;
                _processed += 1;

                if (_processed < _maxCount && _timer.Elapsed < _timeout)
                    return false;

                LogContext.Debug?.Log("Partition: {PartitionId} updating checkpoint with offset: {Offset}", _current.Partition, _current.Offset);
                _consumer.Commit(_current);
                Reset();
                return true;
            }

            public void Close(TopicPartitionOffset partition)
            {
                try
                {
                    if (_current == default)
                        return;

                    LogContext.Debug?.Log("Partition: {PartitionId} updating checkpoint with offset: {Offset}", _current.Partition, _current.Offset);
                    _consumer.Commit(_current);
                }
                finally
                {
                    _timer.Stop();
                    _current = null;

                    LogContext.Info?.Log("Partition: {PartitionId} was closed", partition.TopicPartition);
                }
            }
        }
    }
}
