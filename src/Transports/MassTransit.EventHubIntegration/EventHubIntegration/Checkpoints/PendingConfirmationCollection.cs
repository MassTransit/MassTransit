namespace MassTransit.EventHubIntegration.Checkpoints
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using Azure.Messaging.EventHubs.Processor;


    public class PendingConfirmationCollection :
        IDisposable
    {
        readonly CancellationToken _cancellationToken;
        readonly ConcurrentDictionary<PartitionOffset, IPendingConfirmation> _confirmations;
        readonly CancellationTokenRegistration? _registration;

        public PendingConfirmationCollection(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _confirmations = new ConcurrentDictionary<PartitionOffset, IPendingConfirmation>();

            if (cancellationToken.CanBeCanceled)
                _registration = cancellationToken.Register(Cancel);
        }

        public void Dispose()
        {
            _registration?.Dispose();
        }

        public IPendingConfirmation Add(ProcessEventArgs eventArgs)
        {
            _cancellationToken.ThrowIfCancellationRequested();

            var pendingConfirmation = new PendingConfirmation(eventArgs);
            return _confirmations.AddOrUpdate(eventArgs, key => pendingConfirmation, (key, existing) =>
            {
                existing.Faulted($"Duplicate key: {key} on EventHub: {eventArgs.Partition.EventHubName}");

                return pendingConfirmation;
            });
        }

        public void Faulted(PartitionOffset partitionOffset, Exception exception)
        {
            if (_confirmations.TryRemove(partitionOffset, out var confirmation))
                confirmation.Faulted(exception);
        }

        public void Complete(PartitionOffset partitionOffset)
        {
            if (_confirmations.TryRemove(partitionOffset, out var confirmation))
                confirmation.Complete();
        }

        public void Canceled(PartitionOffset partitionOffset, CancellationToken cancellationToken)
        {
            if (_confirmations.TryRemove(partitionOffset, out var confirmation))
                confirmation.Canceled(cancellationToken);
        }

        void Cancel()
        {
            foreach (var partitionOffset in _confirmations.Keys)
                Canceled(partitionOffset, _cancellationToken);
        }
    }
}
