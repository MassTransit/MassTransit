namespace MassTransit.AzureServiceBusTransport.Topology
{
    using System.Collections.Generic;
    using System.Linq;
    using Azure.Messaging.ServiceBus.Administration;


    public class QueueEntity :
        Queue,
        QueueHandle
    {
        public QueueEntity(long id, CreateQueueOptions createQueueOptions)
        {
            Id = id;
            CreateQueueOptions = createQueueOptions;
        }

        public static IEqualityComparer<QueueEntity> NameComparer { get; } = new NameEqualityComparer();
        public static IEqualityComparer<QueueEntity> EntityComparer { get; } = new QueueEntityEqualityComparer();

        public CreateQueueOptions CreateQueueOptions { get; }
        public long Id { get; }
        public Queue Queue => this;

        public override string ToString()
        {
            return string.Join(", ", new[] { $"path: {CreateQueueOptions.Name}" }.Where(x => !string.IsNullOrWhiteSpace(x)));
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
                return string.Equals(x.CreateQueueOptions.Name, y.CreateQueueOptions.Name)
                    && x.CreateQueueOptions.AutoDeleteOnIdle == y.CreateQueueOptions.AutoDeleteOnIdle
                    && x.CreateQueueOptions.DefaultMessageTimeToLive == y.CreateQueueOptions.DefaultMessageTimeToLive
                    && x.CreateQueueOptions.DuplicateDetectionHistoryTimeWindow == y.CreateQueueOptions.DuplicateDetectionHistoryTimeWindow
                    && x.CreateQueueOptions.EnableBatchedOperations == y.CreateQueueOptions.EnableBatchedOperations
                    && x.CreateQueueOptions.DeadLetteringOnMessageExpiration == y.CreateQueueOptions.DeadLetteringOnMessageExpiration
                    && x.CreateQueueOptions.EnablePartitioning == y.CreateQueueOptions.EnablePartitioning
                    && string.Equals(x.CreateQueueOptions.ForwardDeadLetteredMessagesTo, y.CreateQueueOptions.ForwardDeadLetteredMessagesTo)
                    && string.Equals(x.CreateQueueOptions.ForwardTo, y.CreateQueueOptions.ForwardTo)
                    && x.CreateQueueOptions.LockDuration == y.CreateQueueOptions.LockDuration
                    && x.CreateQueueOptions.MaxDeliveryCount == y.CreateQueueOptions.MaxDeliveryCount
                    && x.CreateQueueOptions.MaxSizeInMegabytes == y.CreateQueueOptions.MaxSizeInMegabytes
                    && x.CreateQueueOptions.RequiresDuplicateDetection == y.CreateQueueOptions.RequiresDuplicateDetection
                    && x.CreateQueueOptions.RequiresSession == y.CreateQueueOptions.RequiresSession
                    && string.Equals(x.CreateQueueOptions.UserMetadata, y.CreateQueueOptions.UserMetadata);
            }

            public int GetHashCode(QueueEntity obj)
            {
                unchecked
                {
                    var hashCode = obj.CreateQueueOptions.Name.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.CreateQueueOptions.AutoDeleteOnIdle.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.CreateQueueOptions.DefaultMessageTimeToLive.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.CreateQueueOptions.DuplicateDetectionHistoryTimeWindow.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.CreateQueueOptions.EnableBatchedOperations.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.CreateQueueOptions.DeadLetteringOnMessageExpiration.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.CreateQueueOptions.EnablePartitioning.GetHashCode();
                    if (!string.IsNullOrWhiteSpace(obj.CreateQueueOptions.ForwardDeadLetteredMessagesTo))
                        hashCode = (hashCode * 397) ^ obj.CreateQueueOptions.ForwardDeadLetteredMessagesTo.GetHashCode();
                    if (!string.IsNullOrWhiteSpace(obj.CreateQueueOptions.ForwardTo))
                        hashCode = (hashCode * 397) ^ obj.CreateQueueOptions.ForwardTo.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.CreateQueueOptions.LockDuration.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.CreateQueueOptions.MaxDeliveryCount.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.CreateQueueOptions.RequiresDuplicateDetection.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.CreateQueueOptions.RequiresSession.GetHashCode();
                    if (!string.IsNullOrWhiteSpace(obj.CreateQueueOptions.UserMetadata))
                        hashCode = (hashCode * 397) ^ obj.CreateQueueOptions.UserMetadata.GetHashCode();

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
                return string.Equals(x.CreateQueueOptions.Name, y.CreateQueueOptions.Name);
            }

            public int GetHashCode(QueueEntity obj)
            {
                return obj.CreateQueueOptions.Name.GetHashCode();
            }
        }
    }
}
