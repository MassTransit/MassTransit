namespace MassTransit.KafkaIntegration.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Configuration;
    using Confluent.Kafka;
    using Context;
    using Util;


    public class ConsumerLockContext<TKey, TValue> :
        IConsumerLockContext<TKey, TValue>
        where TValue : class
    {
        readonly SingleThreadedDictionary<Partition, PartitionCheckpointData> _data = new SingleThreadedDictionary<Partition, PartitionCheckpointData>();
        readonly IHostConfiguration _hostConfiguration;
        readonly ushort _maxCount;
        readonly TimeSpan _timeout;

        public ConsumerLockContext(IHostConfiguration hostConfiguration, ReceiveSettings receiveSettings)
        {
            _hostConfiguration = hostConfiguration;
            _timeout = receiveSettings.CheckpointInterval;
            _maxCount = receiveSettings.CheckpointMessageCount;
        }

        public Task Complete(ConsumeResult<TKey, TValue> result)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            if (_data.TryGetValue(result.Partition, out var data))
                data.TryCheckpoint(result);

            return TaskUtil.Completed;
        }

        public void OnAssigned(IConsumer<TKey, TValue> consumer, IEnumerable<TopicPartition> partitions)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            foreach (var partition in partitions)
            {
                if (_data.TryAdd(partition.Partition, p => new PartitionCheckpointData(partition, consumer, _timeout, _maxCount)))
                    LogContext.Info?.Log("Partition: {PartitionId} was assigned", partition);
            }
        }

        public void OnUnAssigned(IConsumer<TKey, TValue> consumer, IEnumerable<TopicPartitionOffset> partitions)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            foreach (var partition in partitions)
            {
                if (_data.TryRemove(partition.Partition, out var data))
                    data.Close(partition);
            }
        }


        sealed class PartitionCheckpointData
        {
            readonly IConsumer<TKey, TValue> _consumer;
            readonly object _lock;
            readonly int _maxCount;
            readonly TopicPartition _partition;
            readonly TimeSpan _timeout;
            readonly Stopwatch _timer;
            bool _commitIsRequired;
            Offset _offset;
            ushort _processed;

            public PartitionCheckpointData(TopicPartition partition, IConsumer<TKey, TValue> consumer, TimeSpan timeout, ushort maxCount)
            {
                _partition = partition;
                _consumer = consumer;
                _timeout = timeout;
                _maxCount = maxCount;
                _processed = 0;
                _timer = Stopwatch.StartNew();
                _lock = new object();
                _commitIsRequired = false;
            }

            public bool TryCheckpoint(ConsumeResult<TKey, TValue> result)
            {
                void Reset()
                {
                    _processed = 0;
                    _commitIsRequired = false;
                    _timer.Restart();
                }

                lock (_lock)
                {
                    if (_offset < result.Offset)
                    {
                        _offset = result.Offset;
                        _commitIsRequired = true;
                    }

                    _processed += 1;

                    if (_processed < _maxCount && _timer.Elapsed < _timeout)
                        return false;

                    CommitIfRequired();
                    Reset();
                    return true;
                }
            }

            public void Close(TopicPartitionOffset partition)
            {
                try
                {
                    lock (_lock)
                        CommitIfRequired();
                }
                finally
                {
                    _timer.Stop();
                    _offset = default;
                    _commitIsRequired = false;

                    LogContext.Info?.Log("Partition: {PartitionId} was closed", partition.TopicPartition);
                }
            }

            void CommitIfRequired()
            {
                if (!_commitIsRequired)
                    return;

                var offset = _offset + 1;
                LogContext.Debug?.Log("Partition: {PartitionId} updating checkpoint with offset: {Offset}", _partition, offset);
                _consumer.Commit(new[] {new TopicPartitionOffset(_partition, offset)});
            }
        }
    }
}
