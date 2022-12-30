namespace MassTransit.KafkaIntegration.Checkpoints
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using Confluent.Kafka;


    public class PendingConfirmationCollection :
        IDisposable
    {
        readonly ConcurrentDictionary<Offset, IPendingConfirmation> _confirmations;
        readonly TopicPartition _partition;
        readonly CancellationTokenRegistration? _registration;

        public PendingConfirmationCollection(TopicPartition partition, CancellationToken cancellationToken)
        {
            _partition = partition;
            if (!cancellationToken.CanBeCanceled)
                _registration = cancellationToken.Register(() => Cancel(cancellationToken));
            _confirmations = new ConcurrentDictionary<Offset, IPendingConfirmation>();
        }

        public IPendingConfirmation Add(Offset offset)
        {
            var pendingConfirmation = new PendingConfirmation(_partition, offset);
            return _confirmations.AddOrUpdate(offset, key => pendingConfirmation, (key, existing) =>
            {
                existing.Faulted($"Duplicate key: {key}, partition: {_partition}");

                return pendingConfirmation;
            });
        }

        public void Complete(Offset offset)
        {
            if (_confirmations.TryRemove(offset, out var confirmation))
                confirmation.Complete();
        }

        public void Faulted(Offset offset, Exception exception)
        {
            if (_confirmations.TryRemove(offset, out var confirmation))
                confirmation.Faulted(exception);
        }

        void Cancel(CancellationToken cancellationToken)
        {
            foreach (var offset in _confirmations.Keys)
            {
                if (_confirmations.TryRemove(offset, out var confirmation))
                    confirmation.Canceled(cancellationToken);
            }
        }

        public void Dispose()
        {
            _registration?.Dispose();
        }
    }
}
