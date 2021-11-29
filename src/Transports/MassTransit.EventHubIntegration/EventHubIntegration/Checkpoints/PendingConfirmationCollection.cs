namespace MassTransit.EventHubIntegration.Checkpoints
{
    using System;
    using System.Collections.Concurrent;
    using Azure.Messaging.EventHubs.Processor;


    public class PendingConfirmationCollection
    {
        readonly ConcurrentDictionary<long, IPendingConfirmation> _confirmations;
        readonly string _eventHubName;

        public PendingConfirmationCollection(string eventHubName)
        {
            _eventHubName = eventHubName;
            _confirmations = new ConcurrentDictionary<long, IPendingConfirmation>();
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
    }
}
