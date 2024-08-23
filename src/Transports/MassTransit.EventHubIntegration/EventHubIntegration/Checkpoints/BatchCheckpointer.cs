namespace MassTransit.EventHubIntegration.Checkpoints
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Internals;


    public class BatchCheckpointer :
        ICheckpointer
    {
        readonly Channel<IPendingConfirmation> _channel;
        readonly Task _checkpointTask;
        readonly ReceiveSettings _settings;
        readonly CancellationToken _cancellationToken;

        public BatchCheckpointer(ReceiveSettings settings, CancellationToken cancellationToken)
        {
            _settings = settings;
            _cancellationToken = cancellationToken;
            var channelOptions = new BoundedChannelOptions(settings.CheckpointMessageLimit)
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

                await Checkpoint(batch).ConfigureAwait(false);
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

        async Task Checkpoint(List<IPendingConfirmation> batch)
        {
            for (var i = batch.Count - 1; i >= 0; i--)
            {
                if (await TryCheckpoint(batch[i]).ConfigureAwait(false) == false)
                    continue;

                batch.RemoveRange(0, i + 1);
                return;
            }
        }

        async Task<bool> TryCheckpoint(IPendingConfirmation confirmation)
        {
            _cancellationToken.ThrowIfCancellationRequested();

            LogContext.Debug?.Log("Partition: {PartitionId} updating checkpoint with offset: {Offset}", confirmation.Partition.PartitionId,
                confirmation.Offset);

            try
            {
                await confirmation.Checkpoint(_cancellationToken).ConfigureAwait(false);
                return true;
            }
            catch (Exception exception)
            {
                LogContext.Error?.Log(exception, "Partition: {PartitionId} checkpoint failed with offset: {Offset}", confirmation.Partition,
                    confirmation.Offset);
                confirmation.Faulted(exception);
                return false;
            }
        }
    }
}
