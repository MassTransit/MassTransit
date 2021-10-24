namespace MassTransit.Azure.ServiceBus.Core.Topology.Entities
{
    using System.Collections.Generic;
    using System.Linq;
    using global::Azure.Messaging.ServiceBus.Administration;


    public class QueueEntity :
        Queue,
        QueueHandle
    {
        public QueueEntity(long id, CreateQueueOptions queueDescription)
        {
            Id = id;
            QueueDescription = queueDescription;
        }

        public static IEqualityComparer<QueueEntity> NameComparer { get; } = new NameEqualityComparer();
        public static IEqualityComparer<QueueEntity> EntityComparer { get; } = new QueueEntityEqualityComparer();

        public CreateQueueOptions QueueDescription { get; }
        public long Id { get; }
        public Queue Queue => this;

        public override string ToString()
        {
            return string.Join(", ", new[] {$"path: {QueueDescription.Name}"}.Where(x => !string.IsNullOrWhiteSpace(x)));
        }


        sealed class QueueEntityEqualityComparer :
            IEqualityComparer<QueueEntity>
        {
            public bool Equals(QueueEntity x, QueueEntity y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null))
                    return false;
                if (ReferenceEquals(y, null))
                    return false;
                if (x.GetType() != y.GetType())
                    return false;
                return string.Equals(x.QueueDescription.Name, y.QueueDescription.Name)
                    && x.QueueDescription.AutoDeleteOnIdle == y.QueueDescription.AutoDeleteOnIdle
                    && x.QueueDescription.DefaultMessageTimeToLive == y.QueueDescription.DefaultMessageTimeToLive
                    && x.QueueDescription.DuplicateDetectionHistoryTimeWindow == y.QueueDescription.DuplicateDetectionHistoryTimeWindow
                    && x.QueueDescription.EnableBatchedOperations == y.QueueDescription.EnableBatchedOperations
                    && x.QueueDescription.DeadLetteringOnMessageExpiration == y.QueueDescription.DeadLetteringOnMessageExpiration
                    && x.QueueDescription.EnablePartitioning == y.QueueDescription.EnablePartitioning
                    && string.Equals(x.QueueDescription.ForwardDeadLetteredMessagesTo, y.QueueDescription.ForwardDeadLetteredMessagesTo)
                    && string.Equals(x.QueueDescription.ForwardTo, y.QueueDescription.ForwardTo)
                    && x.QueueDescription.LockDuration == y.QueueDescription.LockDuration
                    && x.QueueDescription.MaxDeliveryCount == y.QueueDescription.MaxDeliveryCount
                    && x.QueueDescription.MaxSizeInMegabytes == y.QueueDescription.MaxSizeInMegabytes
                    && x.QueueDescription.RequiresDuplicateDetection == y.QueueDescription.RequiresDuplicateDetection
                    && x.QueueDescription.RequiresSession == y.QueueDescription.RequiresSession
                    && string.Equals(x.QueueDescription.UserMetadata, y.QueueDescription.UserMetadata);
            }

            public int GetHashCode(QueueEntity obj)
            {
                unchecked
                {
                    var hashCode = obj.QueueDescription.Name.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.QueueDescription.AutoDeleteOnIdle.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.QueueDescription.DefaultMessageTimeToLive.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.QueueDescription.DuplicateDetectionHistoryTimeWindow.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.QueueDescription.EnableBatchedOperations.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.QueueDescription.DeadLetteringOnMessageExpiration.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.QueueDescription.EnablePartitioning.GetHashCode();
                    if (!string.IsNullOrWhiteSpace(obj.QueueDescription.ForwardDeadLetteredMessagesTo))
                        hashCode = (hashCode * 397) ^ obj.QueueDescription.ForwardDeadLetteredMessagesTo.GetHashCode();
                    if (!string.IsNullOrWhiteSpace(obj.QueueDescription.ForwardTo))
                        hashCode = (hashCode * 397) ^ obj.QueueDescription.ForwardTo.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.QueueDescription.LockDuration.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.QueueDescription.MaxDeliveryCount.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.QueueDescription.RequiresDuplicateDetection.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.QueueDescription.RequiresSession.GetHashCode();
                    if (!string.IsNullOrWhiteSpace(obj.QueueDescription.UserMetadata))
                        hashCode = (hashCode * 397) ^ obj.QueueDescription.UserMetadata.GetHashCode();

                    return hashCode;
                }
            }
        }


        sealed class NameEqualityComparer :
            IEqualityComparer<QueueEntity>
        {
            public bool Equals(QueueEntity x, QueueEntity y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null))
                    return false;
                if (ReferenceEquals(y, null))
                    return false;
                if (x.GetType() != y.GetType())
                    return false;
                return string.Equals(x.QueueDescription.Name, y.QueueDescription.Name);
            }

            public int GetHashCode(QueueEntity obj)
            {
                return obj.QueueDescription.Name.GetHashCode();
            }
        }
    }
}
