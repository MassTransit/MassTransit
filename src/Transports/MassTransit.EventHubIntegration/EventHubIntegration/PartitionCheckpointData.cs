namespace MassTransit.EventHubIntegration
{
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Processor;
    using Checkpoints;


    public class PartitionCheckpointData
    {
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly ICheckpointer _checkpointer;
        readonly PendingConfirmationCollection _pending;

        public PartitionCheckpointData(ReceiveSettings settings, PendingConfirmationCollection pending)
        {
            _cancellationTokenSource = new CancellationTokenSource();
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

            await _checkpointer.DisposeAsync().ConfigureAwait(false);

            LogContext.Info?.Log("Partition: {PartitionId} was closed, reason: {Reason}", args.PartitionId, args.Reason);

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}
