namespace MassTransit.Azure.ServiceBus.Core.Topology.Entities
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Azure.ServiceBus.Management;


    public class TopicEntity :
        Topic,
        TopicHandle
    {
        public TopicEntity(long id, TopicDescription topicDescription)
        {
            Id = id;

            TopicDescription = topicDescription;
        }

        public static IEqualityComparer<TopicEntity> NameComparer { get; } = new NameEqualityComparer();
        public static IEqualityComparer<TopicEntity> EntityComparer { get; } = new TopicEntityEqualityComparer();

        public TopicDescription TopicDescription { get; }
        public long Id { get; }
        public Topic Topic => this;

        public override string ToString()
        {
            return string.Join(", ", new[] {$"path: {TopicDescription.Path}"}.Where(x => !string.IsNullOrWhiteSpace(x)));
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
                return string.Equals(x.TopicDescription.Path, y.TopicDescription.Path)
                    && x.TopicDescription.AutoDeleteOnIdle == y.TopicDescription.AutoDeleteOnIdle
                    && x.TopicDescription.DefaultMessageTimeToLive == y.TopicDescription.DefaultMessageTimeToLive
                    && x.TopicDescription.DuplicateDetectionHistoryTimeWindow == y.TopicDescription.DuplicateDetectionHistoryTimeWindow
                    && x.TopicDescription.EnableBatchedOperations == y.TopicDescription.EnableBatchedOperations
                    && x.TopicDescription.EnablePartitioning == y.TopicDescription.EnablePartitioning
                    && x.TopicDescription.RequiresDuplicateDetection == y.TopicDescription.RequiresDuplicateDetection
                    && x.TopicDescription.SupportOrdering == y.TopicDescription.SupportOrdering
                    && string.Equals(x.TopicDescription.UserMetadata, y.TopicDescription.UserMetadata);
            }

            public int GetHashCode(TopicEntity obj)
            {
                unchecked
                {
                    var hashCode = obj.TopicDescription.Path.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TopicDescription.AutoDeleteOnIdle.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TopicDescription.DefaultMessageTimeToLive.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TopicDescription.DuplicateDetectionHistoryTimeWindow.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TopicDescription.EnableBatchedOperations.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TopicDescription.EnablePartitioning.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TopicDescription.RequiresDuplicateDetection.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.TopicDescription.SupportOrdering.GetHashCode();
                    if (!string.IsNullOrWhiteSpace(obj.TopicDescription.UserMetadata))
                        hashCode = (hashCode * 397) ^ obj.TopicDescription.UserMetadata.GetHashCode();

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
                return string.Equals(x.TopicDescription.Path, y.TopicDescription.Path);
            }

            public int GetHashCode(TopicEntity obj)
            {
                return obj.TopicDescription.Path.GetHashCode();
            }
        }
    }
}
