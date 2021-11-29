namespace MassTransit.EventHubIntegration.Checkpoints
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Processor;
    using Internals;
    using Util;


    public class BatchCheckpointer :
        ICheckpointer
    {
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly Channel<IPendingConfirmation> _channel;
        readonly Task _checkpointTask;
        readonly ChannelExecutor _executor;
        readonly ReceiveSettings _settings;
        readonly CancellationTokenSource _shutdownTokenSource;

        public BatchCheckpointer(ChannelExecutor executor, ReceiveSettings settings)
        {
            _executor = executor;
            _settings = settings;
            var channelOptions = new BoundedChannelOptions(settings.CheckpointMessageLimit)
            {
                AllowSynchronousContinuations = false,
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = true
            };

            _channel = Channel.CreateBounded<IPendingConfirmation>(channelOptions);
            _checkpointTask = Task.Run(WaitForBatch);
            _cancellationTokenSource = new CancellationTokenSource();
            _shutdownTokenSource = new CancellationTokenSource();
        }

        public async Task Pending(IPendingConfirmation confirmation)
        {
            await _channel.Writer.WriteAsync(confirmation).ConfigureAwait(false);
        }

        public async Task Close(ProcessingStoppedReason stoppedReason)
        {
            _channel.Writer.Complete();

            if (stoppedReason != ProcessingStoppedReason.Shutdown)
                _cancellationTokenSource.Cancel();

            _shutdownTokenSource.Cancel();

            await _checkpointTask.ConfigureAwait(false);

            _cancellationTokenSource.Dispose();
            _shutdownTokenSource.Dispose();
        }

        async Task WaitForBatch()
        {
            try
            {
                while (await _channel.Reader.WaitToReadAsync(_cancellationTokenSource.Token).ConfigureAwait(false))
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
            var batchToken = CancellationTokenSource.CreateLinkedTokenSource(timeoutToken.Token, _cancellationTokenSource.Token, _shutdownTokenSource.Token);
            var batch = new List<IPendingConfirmation>();

            try
            {
                try
                {
                    for (var i = 0; i < _settings.CheckpointMessageCount; i++)
                    {
                        var confirmation = await _channel.Reader.ReadAsync(batchToken.Token).ConfigureAwait(false);

                        await confirmation.Confirmed.OrCanceled(batchToken.Token).ConfigureAwait(false);

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
            _cancellationTokenSource.Token.ThrowIfCancellationRequested();

            LogContext.Debug?.Log("Partition: {PartitionId} updating checkpoint with offset: {Offset}", confirmation.Partition.PartitionId,
                confirmation.Offset);

            try
            {
                await confirmation.Checkpoint(_cancellationTokenSource.Token).ConfigureAwait(false);
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
