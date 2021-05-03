namespace MassTransit.KafkaIntegration.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Confluent.Kafka;
    using Context;
    using GreenPipes;
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

        public async Task Complete(ConsumeResult<TKey, TValue> result)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            if (_data.TryGetValue(result.Partition, out var data))
                await data.TryCheckpoint(result).ConfigureAwait(false);
        }

        public void OnAssigned(IEnumerable<TopicPartition> partitions, IPipe<CheckpointContext> pipe)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            PartitionCheckpointData CreateData(TopicPartition partition)
            {
                return new PartitionCheckpointData(partition, _timeout, _maxCount, pipe);
            }

            foreach (var partition in partitions)
            {
                if (_data.TryAdd(partition.Partition, p => CreateData(partition)))
                    LogContext.Info?.Log("Partition: {PartitionId} was assigned", partition);
            }
        }

        public void OnUnAssigned(IConsumer<TKey, TValue> consumer, IEnumerable<TopicPartitionOffset> partitions)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            foreach (var partition in partitions)
            {
                if (_data.TryRemove(partition.Partition, out var data))
                    TaskUtil.Await(() => data.Close(partition));
            }
        }


        sealed class PartitionCheckpointData
        {
            readonly SemaphoreSlim _lock;
            readonly int _maxCount;
            readonly TopicPartition _partition;
            readonly IPipe<CheckpointContext> _pipe;
            readonly TimeSpan _timeout;
            readonly Stopwatch _timer;
            bool _commitIsRequired;
            Offset _offset;
            ushort _processed;

            public PartitionCheckpointData(TopicPartition partition, TimeSpan timeout, ushort maxCount, IPipe<CheckpointContext> pipe)
            {
                _partition = partition;
                _timeout = timeout;
                _maxCount = maxCount;
                _pipe = pipe;
                _processed = 0;
                _timer = Stopwatch.StartNew();
                _lock = new SemaphoreSlim(1);
                _commitIsRequired = false;
            }

            public async Task TryCheckpoint(ConsumeResult<TKey, TValue> result)
            {
                void Reset()
                {
                    _processed = 0;
                    _commitIsRequired = false;
                    _timer.Restart();
                }

                try
                {
                    await _lock.WaitAsync().ConfigureAwait(false);

                    if (_offset < result.Offset)
                    {
                        _offset = result.Offset;
                        _commitIsRequired = true;
                    }

                    _processed += 1;

                    if (_processed < _maxCount && _timer.Elapsed < _timeout)
                        return;

                    await CommitIfRequired().ConfigureAwait(false);
                    Reset();
                }
                finally
                {
                    _lock.Release();
                }
            }

            public async Task Close(TopicPartitionOffset partition)
            {
                try
                {
                    await _lock.WaitAsync().ConfigureAwait(false);
                    await CommitIfRequired().ConfigureAwait(false);
                }
                finally
                {
                    _timer.Stop();
                    _offset = default;
                    _commitIsRequired = false;
                    _lock.Dispose();

                    LogContext.Info?.Log("Partition: {PartitionId} was closed", partition.TopicPartition);
                }
            }

            Task CommitIfRequired()
            {
                return !_commitIsRequired ? TaskUtil.Completed : _pipe.Send(new KafkaCheckpointContext(_partition, _offset + 1));
            }


            class KafkaCheckpointContext :
                BasePipeContext,
                CheckpointContext
            {
                public KafkaCheckpointContext(TopicPartition partition, Offset offset)
                {
                    Topic = partition.Topic;
                    Partition = partition.Partition;
                    Offset = offset;
                }

                public string Topic { get; }
                public int Partition { get; }
                public long Offset { get; }
            }
        }
    }
}
