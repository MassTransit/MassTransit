namespace MassTransit.KafkaIntegration.Checkpoints
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Internals;


    public class BatchCheckpointer :
        ICheckpointer
    {
        readonly Channel<IPendingConfirmation> _channel;
        readonly Task _checkpointTask;
        readonly IConsumer<byte[], byte[]> _consumer;
        readonly ReceiveSettings _settings;
        readonly CancellationToken _cancellationToken;

        public BatchCheckpointer(IConsumer<byte[], byte[]> consumer, ReceiveSettings settings, CancellationToken cancellationToken)
        {
            _consumer = consumer;
            _settings = settings;
            _cancellationToken = cancellationToken;
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
            _channel.Writer.TryComplete();

            await _checkpointTask.ConfigureAwait(false);
        }

        async Task WaitForBatch()
        {
            try
            {
                while (await _channel.Reader.WaitToReadAsync(_cancellationToken).ConfigureAwait(false))
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
            var timeoutToken = new CancellationTokenSource(_settings.CheckpointInterval);
            var batchToken = CancellationTokenSource.CreateLinkedTokenSource(timeoutToken.Token, _cancellationToken);
            var batch = new List<IPendingConfirmation>(_settings.CheckpointMessageCount);

            try
            {
                try
                {
                    while (batch.Count < _settings.CheckpointMessageCount)
                    {
                        if (_channel.Reader.TryRead(out var confirmation))
                        {
                            await confirmation.Confirmed.OrCanceled(_cancellationToken).ConfigureAwait(false);
                            batch.Add(confirmation);
                        }
                        else if (await _channel.Reader.WaitToReadAsync(batchToken.Token).ConfigureAwait(false) == false)
                        {
                            break;
                        }
                    }
                }
                catch (Exception) when (batch.Count > 0)
                {
                }

                Checkpoint(batch);
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
                timeoutToken.Dispose();
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
            _cancellationToken.ThrowIfCancellationRequested();

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
