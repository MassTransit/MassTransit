namespace MassTransit.EventHubIntegration.Checkpoints
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using Azure.Messaging.EventHubs.Processor;


    public class PendingConfirmationCollection :
        IDisposable
    {
        readonly ConcurrentDictionary<long, IPendingConfirmation> _confirmations;
        readonly string _eventHubName;
        readonly CancellationTokenRegistration? _registration;

        public PendingConfirmationCollection(string eventHubName, CancellationToken cancellationToken)
        {
            _eventHubName = eventHubName;
            _confirmations = new ConcurrentDictionary<long, IPendingConfirmation>();

            if (cancellationToken.CanBeCanceled)
                _registration = cancellationToken.Register(() => Cancel(cancellationToken));
        }

        public void Dispose()
        {
            _registration?.Dispose();
        }

        public IPendingConfirmation Add(ProcessEventArgs eventArgs)
        {
            var pendingConfirmation = new PendingConfirmation(_eventHubName, eventArgs);
            return _confirmations.AddOrUpdate(pendingConfirmation.Offset, key => pendingConfirmation, (key, existing) =>
            {
                existing.Faulted($"Duplicate key: {key}, partition: {eventArgs.Partition.PartitionId}");

                return pendingConfirmation;
            });
        }

        public void Faulted(long offset, Exception exception)
        {
            if (_confirmations.TryRemove(offset, out var confirmation))
                confirmation.Faulted(exception);
        }

        public void Complete(long offset)
        {
            if (_confirmations.TryRemove(offset, out var confirmation))
                confirmation.Complete();
        }

        void Cancel(CancellationToken cancellationToken)
        {
            foreach (var offset in _confirmations.Keys)
            {
                if (_confirmations.TryRemove(offset, out var confirmation))
                    confirmation.Canceled(cancellationToken);
            }
        }
    }
}
