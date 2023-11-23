namespace MassTransit.KafkaIntegration.Checkpoints
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using Confluent.Kafka;


    public class PendingConfirmationCollection :
        IDisposable
    {
        readonly CancellationToken _cancellationToken;
        readonly ConcurrentDictionary<TopicPartitionOffset, IPendingConfirmation> _confirmations;
        readonly CancellationTokenRegistration? _registration;

        public PendingConfirmationCollection(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _confirmations = new ConcurrentDictionary<TopicPartitionOffset, IPendingConfirmation>();

            if (cancellationToken.CanBeCanceled)
                _registration = cancellationToken.Register(() => Cancel());
        }

        public void Dispose()
        {
            _registration?.Dispose();
        }

        public IPendingConfirmation Add(TopicPartitionOffset offset)
        {
            _cancellationToken.ThrowIfCancellationRequested();

            var pendingConfirmation = new PendingConfirmation(offset.TopicPartition, offset.Offset);
            return _confirmations.AddOrUpdate(offset, key => pendingConfirmation, (key, existing) =>
            {
                existing.Faulted($"Duplicate key: {key}");

                return pendingConfirmation;
            });
        }

        public void Complete(TopicPartitionOffset offset)
        {
            if (_confirmations.TryRemove(offset, out var confirmation))
                confirmation.Complete();
        }

        public void Canceled(TopicPartitionOffset offset, CancellationToken cancellationToken)
        {
            if (_confirmations.TryRemove(offset, out var confirmation))
                confirmation.Canceled(cancellationToken);
        }

        public void Faulted(TopicPartitionOffset offset, Exception exception)
        {
            if (_confirmations.TryRemove(offset, out var confirmation))
                confirmation.Faulted(exception);
        }

        void Cancel()
        {
            foreach (var offset in _confirmations.Keys)
                Canceled(offset, _cancellationToken);
        }
    }
}
