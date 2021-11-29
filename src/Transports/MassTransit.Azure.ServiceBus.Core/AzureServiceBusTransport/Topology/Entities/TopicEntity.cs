namespace MassTransit.AzureServiceBusTransport.Topology
{
    using System.Collections.Generic;
    using System.Linq;
    using Azure.Messaging.ServiceBus.Administration;


    public class TopicEntity :
        Topic,
        TopicHandle
    {
        public TopicEntity(long id, CreateTopicOptions createTopicOptions)
        {
            Id = id;

            CreateTopicOptions = createTopicOptions;
        }

        public static IEqualityComparer<TopicEntity> NameComparer { get; } = new NameEqualityComparer();
        public static IEqualityComparer<TopicEntity> EntityComparer { get; } = new TopicEntityEqualityComparer();

        public CreateTopicOptions CreateTopicOptions { get; }
        public long Id { get; }
        public Topic Topic => this;

        public override string ToString()
        {
            return string.Join(", ", new[] { $"path: {CreateTopicOptions.Name}" }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }


        sealed class TopicEntityEqualityComparer :
            IEqualityComparer<TopicEntity>
        {
            public bool Equals(TopicEntity x, TopicEntity y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null))
                    return false;
                if (ReferenceEquals(y, null))
                    return false;
                if (x.GetType() != y.GetType())
                    return false;
                return string.Equals(x.CreateTopicOptions.Name, y.CreateTopicOptions.Name)
                    && x.CreateTopicOptions.AutoDeleteOnIdle == y.CreateTopicOptions.AutoDeleteOnIdle
                    && x.CreateTopicOptions.DefaultMessageTimeToLive == y.CreateTopicOptions.DefaultMessageTimeToLive
                    && x.CreateTopicOptions.DuplicateDetectionHistoryTimeWindow == y.CreateTopicOptions.DuplicateDetectionHistoryTimeWindow
                    && x.CreateTopicOptions.EnableBatchedOperations == y.CreateTopicOptions.EnableBatchedOperations
                    && x.CreateTopicOptions.EnablePartitioning == y.CreateTopicOptions.EnablePartitioning
                    && x.CreateTopicOptions.RequiresDuplicateDetection == y.CreateTopicOptions.RequiresDuplicateDetection
                    && x.CreateTopicOptions.SupportOrdering == y.CreateTopicOptions.SupportOrdering
                    && string.Equals(x.CreateTopicOptions.UserMetadata, y.CreateTopicOptions.UserMetadata);
            }

            public int GetHashCode(TopicEntity obj)
            {
                unchecked
                {
                    var hashCode = obj.CreateTopicOptions.Name.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.CreateTopicOptions.AutoDeleteOnIdle.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.CreateTopicOptions.DefaultMessageTimeToLive.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.CreateTopicOptions.DuplicateDetectionHistoryTimeWindow.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.CreateTopicOptions.EnableBatchedOperations.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.CreateTopicOptions.EnablePartitioning.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.CreateTopicOptions.RequiresDuplicateDetection.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.CreateTopicOptions.SupportOrdering.GetHashCode();
                    if (!string.IsNullOrWhiteSpace(obj.CreateTopicOptions.UserMetadata))
                        hashCode = (hashCode * 397) ^ obj.CreateTopicOptions.UserMetadata.GetHashCode();

                    return hashCode;
                }
            }
        }


        sealed class NameEqualityComparer :
            IEqualityComparer<TopicEntity>
        {
            public bool Equals(TopicEntity x, TopicEntity y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null))
                    return false;
                if (ReferenceEquals(y, null))
                    return false;
                if (x.GetType() != y.GetType())
                    return false;
                return string.Equals(x.CreateTopicOptions.Name, y.CreateTopicOptions.Name);
            }

            public int GetHashCode(TopicEntity obj)
            {
                return obj.CreateTopicOptions.Name.GetHashCode();
            }
        }
    }
}
