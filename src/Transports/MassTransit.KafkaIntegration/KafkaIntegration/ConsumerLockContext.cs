namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Checkpoints;
    using Confluent.Kafka;
    using MassTransit.Configuration;
    using Util;


    public class ConsumerLockContext<TKey, TValue> :
        IConsumerLockContext<TKey, TValue>
        where TValue : class
    {
        readonly SingleThreadedDictionary<Partition, PartitionCheckpointData> _data = new SingleThreadedDictionary<Partition, PartitionCheckpointData>();
        readonly IHostConfiguration _hostConfiguration;
        readonly ReceiveSettings _receiveSettings;

        public ConsumerLockContext(IHostConfiguration hostConfiguration, ReceiveSettings receiveSettings)
        {
            _hostConfiguration = hostConfiguration;
            _receiveSettings = receiveSettings;
        }

        public async Task Pending(ConsumeResult<TKey, TValue> result)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            if (_data.TryGetValue(result.Partition, out var data))
                await data.Pending(result).ConfigureAwait(false);
        }

        public Task Complete(ConsumeResult<TKey, TValue> result)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            if (_data.TryGetValue(result.Partition, out var data))
                data.Complete(result);

            return Task.CompletedTask;
        }

        public Task Faulted(ConsumeResult<TKey, TValue> result, Exception exception)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            if (_data.TryGetValue(result.Partition, out var data))
                data.Faulted(result, exception);

            return Task.CompletedTask;
        }

        public void OnAssigned(IConsumer<TKey, TValue> consumer, IEnumerable<TopicPartition> partitions)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            foreach (var partition in partitions)
            {
                if (_data.TryAdd(partition.Partition, p => new PartitionCheckpointData(partition, consumer, _receiveSettings)))
                    LogContext.Info?.Log("Partition: {PartitionId} was assigned", partition);
            }
        }

        public void OnUnAssigned(IConsumer<TKey, TValue> consumer, IEnumerable<TopicPartitionOffset> partitions)
        {
            LogContext.SetCurrentIfNull(_hostConfiguration.ReceiveLogContext);

            var tasks = new List<Task>();
            foreach (var partition in partitions)
            {
                if (_data.TryRemove(partition.Partition, out var data))
                    tasks.Add(data.Close(partition));
            }

            async Task Close()
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }

            TaskUtil.Await(Close);
        }


        sealed class PartitionCheckpointData
        {
            readonly ICheckpointer _checkpointer;
            readonly ChannelExecutor _executor;
            readonly PendingConfirmationCollection _pending;

            public PartitionCheckpointData(TopicPartition partition, IConsumer<TKey, TValue> consumer, ReceiveSettings settings)
            {
                _pending = new PendingConfirmationCollection(partition);
                _executor = new ChannelExecutor(1);
                _checkpointer = new BatchCheckpointer<TKey, TValue>(_executor, consumer, settings);
            }

            public Task Pending(ConsumeResult<TKey, TValue> result)
            {
                var pendingConfirmation = _pending.Add(result.Offset);
                return _checkpointer.Pending(pendingConfirmation);
            }

            public void Complete(ConsumeResult<TKey, TValue> result)
            {
                _pending.Complete(result.Offset);
            }

            public void Faulted(ConsumeResult<TKey, TValue> result, Exception exception)
            {
                _pending.Faulted(result.Offset, exception);
            }

            public async Task Close(TopicPartitionOffset partition)
            {
                await _checkpointer.DisposeAsync().ConfigureAwait(false);
                await _executor.DisposeAsync().ConfigureAwait(false);

                LogContext.Info?.Log("Partition: {PartitionId} was closed", partition.TopicPartition);
            }
        }
    }
}
