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
            _pending.Dispose();
            return default;
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
    }
}
