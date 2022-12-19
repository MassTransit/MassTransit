namespace MassTransit.KafkaIntegration.Checkpoints
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Util;


    public class BatchCheckpointer :
        ICheckpointer
    {
        readonly Channel<IPendingConfirmation> _channel;
        readonly Task _checkpointTask;
        readonly IConsumer<byte[], byte[]> _consumer;
        readonly ChannelExecutor _executor;
        readonly ReceiveSettings _settings;

        public BatchCheckpointer(IConsumer<byte[], byte[]> consumer, ReceiveSettings settings)
        {
            _executor = new ChannelExecutor(1);
            _consumer = consumer;
            _settings = settings;
            var channelOptions = new BoundedChannelOptions(settings.MessageLimit)
            {
                AllowSynchronousContinuations = false,
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = true
            };

            _channel = Channel.CreateBounded<IPendingConfirmation>(channelOptions);
            _checkpointTask = Task.Run(WaitForBatch);
        }

        public async Task Pending(IPendingConfirmation confirmation)
        {
            await _channel.Writer.WriteAsync(confirmation).ConfigureAwait(false);
        }

        public async ValueTask DisposeAsync()
        {
            _channel.Writer.Complete();

            await _checkpointTask.ConfigureAwait(false);

            await _executor.DisposeAsync().ConfigureAwait(false);
        }

        async Task WaitForBatch()
        {
            try
            {
                while (await _channel.Reader.WaitToReadAsync().ConfigureAwait(false))
                    await ReadBatch().ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            catch (ChannelClosedException)
            {
            }
            catch (Exception exception)
            {
                LogContext.Error?.Log(exception, "WaitForBatch Faulted");
            }
        }

        async Task ReadBatch()
        {
            var batchToken = new CancellationTokenSource(_settings.CheckpointInterval);
            var batch = new List<IPendingConfirmation>();

            try
            {
                try
                {
                    for (var i = 0; i < _settings.CheckpointMessageCount; i++)
                    {
                        var confirmation = await _channel.Reader.ReadAsync(batchToken.Token).ConfigureAwait(false);

                        await confirmation.Confirmed.ConfigureAwait(false);

                        batch.Add(confirmation);

                        if (await _channel.Reader.WaitToReadAsync(batchToken.Token).ConfigureAwait(false) == false)
                            break;
                    }
                }
                catch (OperationCanceledException exception) when (exception.CancellationToken == batchToken.Token && batch.Count > 0)
                {
                }

                await _executor.Run(() => Checkpoint(batch)).ConfigureAwait(false);
            }
            catch (OperationCanceledException exception) when (exception.CancellationToken == batchToken.Token)
            {
            }
            catch (Exception exception)
            {
                for (var i = 0; i < batch.Count; i++)
                    batch[i].Faulted(exception);
            }
            finally
            {
                batchToken.Dispose();
            }
        }

        void Checkpoint(List<IPendingConfirmation> batch)
        {
            for (var i = batch.Count - 1; i >= 0; i--)
            {
                if (!TryCheckpoint(batch[i]))
                    continue;

                batch.RemoveRange(0, i + 1);
                return;
            }
        }

        bool TryCheckpoint(IPendingConfirmation confirmation)
        {
            var offset = confirmation.Offset + 1;
            LogContext.Debug?.Log("Partition: {PartitionId} updating checkpoint with offset: {Offset} on {MemberId}", confirmation.Partition, offset,
                _consumer.MemberId);
            try
            {
                _consumer.Commit(new[] { new TopicPartitionOffset(confirmation.Partition, offset) });
                return true;
            }
            catch (KafkaException exception)
            {
                LogContext.Error?.Log(exception, "Partition: {PartitionId} checkpoint failed with offset: {Offset} on {MemberId}", confirmation.Partition,
                    offset, _consumer.MemberId);

                if (exception.Error.IsLocalError)
                    throw;

                confirmation.Faulted(exception);
                return false;
            }
        }
    }
}
