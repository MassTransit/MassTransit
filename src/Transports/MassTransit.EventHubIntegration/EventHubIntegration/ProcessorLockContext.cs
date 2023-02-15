namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Processor;
    using Checkpoints;
    using Util;


    public class ProcessorLockContext :
        IProcessorLockContext,
        ProcessorClientBuilderContext
    {
        readonly CancellationToken _cancellationToken;
        readonly ProcessorContext _context;
        readonly SingleThreadedDictionary<string, PartitionCheckpointData> _data = new SingleThreadedDictionary<string, PartitionCheckpointData>();
        readonly ReceiveSettings _receiveSettings;

        public ProcessorLockContext(ProcessorContext context, ReceiveSettings receiveSettings, CancellationToken cancellationToken)
        {
            _context = context;
            _receiveSettings = receiveSettings;
            _cancellationToken = cancellationToken;
        }

        public Task Pending(ProcessEventArgs eventArgs)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            return _data.TryGetValue(eventArgs.Partition.PartitionId, out var data) ? data.Pending(eventArgs) : Task.CompletedTask;
        }

        public Task Faulted(ProcessEventArgs eventArgs, Exception exception)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            if (_data.TryGetValue(eventArgs.Partition.PartitionId, out var data))
                data.Faulted(eventArgs, exception);

            return Task.CompletedTask;
        }

        public Task Complete(ProcessEventArgs eventArgs)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            if (_data.TryGetValue(eventArgs.Partition.PartitionId, out var data))
                data.Complete(eventArgs);

            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
        }

        public Task Push(ProcessEventArgs partition, Func<Task> method, CancellationToken cancellationToken = default)
        {
            return _data[partition.Partition.PartitionId].Push(method);
        }

        public Task Run(ProcessEventArgs partition, Func<Task> method, CancellationToken cancellationToken = default)
        {
            return _data[partition.Partition.PartitionId].Run(method);
        }

        public Task OnPartitionInitializing(PartitionInitializingEventArgs eventArgs)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            if (_data.TryAdd(eventArgs.PartitionId, _ => new PartitionCheckpointData(_receiveSettings, _cancellationToken)))
                LogContext.Info?.Log("Partition: {PartitionId} was initialized", eventArgs.PartitionId);

            return Task.CompletedTask;
        }

        public async Task OnPartitionClosing(PartitionClosingEventArgs eventArgs)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            if (!_data.TryGetValue(eventArgs.PartitionId, out var data))
                return;

            await data.Close(eventArgs).ConfigureAwait(false);
            _data.TryRemove(eventArgs.PartitionId, out _);
        }


        sealed class PartitionCheckpointData
        {
            readonly CancellationToken _cancellationToken;
            readonly CancellationTokenSource _cancellationTokenSource;
            readonly ICheckpointer _checkpointer;
            readonly ChannelExecutor _executor;
            readonly PendingConfirmationCollection _pending;

            public PartitionCheckpointData(ReceiveSettings settings, CancellationToken cancellationToken)
            {
                _cancellationToken = cancellationToken;
                _cancellationTokenSource = new CancellationTokenSource();
                _executor = new ChannelExecutor(settings.PrefetchCount, settings.ConcurrentMessageLimit);
                _checkpointer = new BatchCheckpointer(settings, _cancellationTokenSource.Token);
                _pending = new PendingConfirmationCollection(settings.EventHubName, cancellationToken);
            }

            public Task Pending(ProcessEventArgs eventArgs)
            {
                if (_cancellationToken.IsCancellationRequested)
                    return Task.CompletedTask;

                var pendingConfirmation = _pending.Add(eventArgs);
                return _checkpointer.Pending(pendingConfirmation);
            }

            public void Complete(ProcessEventArgs eventArgs)
            {
                _pending.Complete(eventArgs.Data.Offset);
            }

            public void Faulted(ProcessEventArgs eventArgs, Exception exception)
            {
                _pending.Faulted(eventArgs.Data.Offset, exception);
            }

            public async Task Close(PartitionClosingEventArgs args)
            {
                if (args.Reason != ProcessingStoppedReason.Shutdown)
                    _cancellationTokenSource.Cancel();

                await _executor.DisposeAsync().ConfigureAwait(false);
                await _checkpointer.DisposeAsync().ConfigureAwait(false);

                LogContext.Info?.Log("Partition: {PartitionId} was closed, reason: {Reason}", args.PartitionId, args.Reason);

                _pending.Dispose();
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }

            public Task Push(Func<Task> method)
            {
                return _executor.Push(method, _cancellationTokenSource.Token);
            }

            public Task Run(Func<Task> method)
            {
                return _executor.Run(method, _cancellationTokenSource.Token);
            }
        }
    }
}
