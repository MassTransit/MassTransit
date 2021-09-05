namespace MassTransit.KafkaIntegration.Checkpoints
{
    using System;
    using System.Collections.Concurrent;
    using Confluent.Kafka;


    public class PendingConfirmationCollection
    {
        readonly ConcurrentDictionary<Offset, IPendingConfirmation> _confirmations;
        readonly TopicPartition _partition;

        public PendingConfirmationCollection(TopicPartition partition)
        {
            _partition = partition;
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
    }
}
