namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Util;


    public abstract class Batcher<TEntry> :
        IBatcher<TEntry>
    {
        readonly Task _batchTask;
        readonly Channel<BatchEntry<TEntry>> _channel;
        readonly ChannelExecutor _executor;
        readonly BatchSettings _settings;

        protected Batcher()
        {
            _settings = ClientContextBatchSettings.GetBatchSettings();

            var channelOptions = new BoundedChannelOptions(_settings.MessageLimit)
            {
                AllowSynchronousContinuations = false,
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            };

            _channel = Channel.CreateBounded<BatchEntry<TEntry>>(channelOptions);
            _executor = new ChannelExecutor(2, _settings.BatchLimit);
            _batchTask = Task.Run(WaitForBatch);
        }

        public async Task Execute(TEntry entry, CancellationToken cancellationToken)
        {
            var batchEntry = new BatchEntry<TEntry>(entry);

            await _channel.Writer.WriteAsync(batchEntry, cancellationToken).ConfigureAwait(false);

            await batchEntry.Completed.ConfigureAwait(false);
        }

        public async ValueTask DisposeAsync()
        {
            _channel.Writer.Complete();

            await _batchTask.ConfigureAwait(false);

            await _executor.DisposeAsync().ConfigureAwait(false);
        }

        async Task WaitForBatch()
        {
            try
            {
                while (await _channel.Reader.WaitToReadAsync().ConfigureAwait(false))
                    await ReadBatch().ConfigureAwait(false);
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
            var batchToken = new CancellationTokenSource(_settings.Timeout);
            var batch = new List<BatchEntry<TEntry>>();
            try
            {
                try
                {
                    for (int entryId = 0,
                        batchLength = 0;
                        entryId < _settings.MessageLimit && batchLength < _settings.SizeLimit;
                        entryId++)
                    {
                        BatchEntry<TEntry> entry = await _channel.Reader.ReadAsync(batchToken.Token).ConfigureAwait(false);

                        batchLength += AddingEntry(entry.Entry, entryId.ToString());
                        batch.Add(entry);

                        if (await _channel.Reader.WaitToReadAsync(batchToken.Token).ConfigureAwait(false) == false)
                            break;
                    }
                }
                catch (OperationCanceledException exception) when (exception.CancellationToken == batchToken.Token && batch.Count > 0)
                {
                }

                await _executor.Push(() => ExecuteBatch(batch)).ConfigureAwait(false);
            }
            catch (OperationCanceledException exception) when (exception.CancellationToken == batchToken.Token)
            {
            }
            catch (Exception exception)
            {
                foreach (BatchEntry<TEntry> entry in batch)
                    entry.SetFaulted(exception);
            }
            finally
            {
                batchToken.Dispose();
            }
        }

        protected abstract int AddingEntry(TEntry entry, string entryId);

        protected abstract Task SendBatch(IList<BatchEntry<TEntry>> batch);

        protected void Complete(IList<BatchEntry<TEntry>> batch, IEnumerable<string> successfulIds)
        {
            foreach (var id in successfulIds)
            {
                if (int.TryParse(id, out var entryId))
                    batch[entryId].SetCompleted();
            }
        }

        protected void Fail(IList<BatchEntry<TEntry>> batch, string id, string code, string message)
        {
            if (int.TryParse(id, out var entryId))
                batch[entryId].SetFaulted(new AmazonSqsTransportException($"Send failed: {code}-{message}"));
        }

        async Task ExecuteBatch(IList<BatchEntry<TEntry>> batch)
        {
            try
            {
                await SendBatch(batch).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                foreach (BatchEntry<TEntry> entry in batch)
                    entry.SetFaulted(exception);
            }
        }
    }
}
