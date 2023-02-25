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
        ProcessorClientBuilderContext,
        IChannelExecutorPool<ProcessEventArgs>
    {
        readonly ProcessorContext _context;
        readonly SingleThreadedDictionary<string, PartitionCheckpointData> _data;
        readonly PendingConfirmationCollection _pending;
        readonly ReceiveSettings _receiveSettings;

        public ProcessorLockContext(ProcessorContext context, ReceiveSettings receiveSettings, CancellationToken cancellationToken)
        {
            _context = context;
            _receiveSettings = receiveSettings;
            _pending = new PendingConfirmationCollection(cancellationToken);
            _data = new SingleThreadedDictionary<string, PartitionCheckpointData>(StringComparer.Ordinal);
        }

        public ValueTask DisposeAsync()
        {
            return default;
        }

        public Task Push(ProcessEventArgs partition, Func<Task> method, CancellationToken cancellationToken = default)
        {
            return _data.TryGetValue(partition.Partition.PartitionId, out var data) ? data.Push(method) : Task.CompletedTask;
        }

        public Task Run(ProcessEventArgs partition, Func<Task> method, CancellationToken cancellationToken = default)
        {
            return _data.TryGetValue(partition.Partition.PartitionId, out var data) ? data.Run(method) : Task.CompletedTask;
        }

        public Task Pending(ProcessEventArgs eventArgs)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            return _data.TryGetValue(eventArgs.Partition.PartitionId, out var data) ? data.Pending(eventArgs) : Task.CompletedTask;
        }

        public Task Faulted(ProcessEventArgs eventArgs, Exception exception)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            _pending.Faulted(eventArgs, exception);

            return Task.CompletedTask;
        }

        public Task Complete(ProcessEventArgs eventArgs)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            _pending.Complete(eventArgs);

            return Task.CompletedTask;
        }

        public void Canceled(ProcessEventArgs eventArgs, CancellationToken cancellationToken)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            _pending.Canceled(eventArgs, cancellationToken);
        }

        public Task OnPartitionInitializing(PartitionInitializingEventArgs eventArgs)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            if (_data.TryAdd(eventArgs.PartitionId, _ => new PartitionCheckpointData(_receiveSettings, _pending)))
                LogContext.Info?.Log("Partition: {PartitionId} was initialized", eventArgs.PartitionId);

            return Task.CompletedTask;
        }

        public Task OnPartitionClosing(PartitionClosingEventArgs eventArgs)
        {
            LogContext.SetCurrentIfNull(_context.LogContext);

            return _data.TryRemove(eventArgs.PartitionId, out var data) ? data.Close(eventArgs) : Task.CompletedTask;
        }


        sealed class PartitionCheckpointData
        {
            readonly CancellationTokenSource _cancellationTokenSource;
            readonly ICheckpointer _checkpointer;
            readonly ChannelExecutor _executor;
            readonly PendingConfirmationCollection _pending;

            public PartitionCheckpointData(ReceiveSettings settings, PendingConfirmationCollection pending)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                _executor = new ChannelExecutor(settings.PrefetchCount, settings.ConcurrentMessageLimit);
                _checkpointer = new BatchCheckpointer(settings, _cancellationTokenSource.Token);
                _pending = pending;
            }

            public Task Pending(ProcessEventArgs eventArgs)
            {
                var pendingConfirmation = _pending.Add(eventArgs);
                return _checkpointer.Pending(pendingConfirmation);
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
