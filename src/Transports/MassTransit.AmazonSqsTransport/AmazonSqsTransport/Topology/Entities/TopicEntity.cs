namespace MassTransit.AmazonSqsTransport.Topology
{
    using System.Collections.Generic;
    using System.Linq;


    public class TopicEntity :
        Topic,
        TopicHandle
    {
        public TopicEntity(long id, string name, bool durable, bool autoDelete, IDictionary<string, object> topicAttributes = null,
            IDictionary<string, object> topicSubscriptionAttributes = null, IDictionary<string, string> topicTags = null)
        {
            Id = id;
            EntityName = name;
            Durable = durable;
            AutoDelete = autoDelete;
            TopicAttributes = topicAttributes ?? new Dictionary<string, object>();
            TopicSubscriptionAttributes = topicSubscriptionAttributes ?? new Dictionary<string, object>();
            TopicTags = topicTags ?? new Dictionary<string, string>();

            EnsureRawDeliveryIsSet();
        }

        public static IEqualityComparer<TopicEntity> NameComparer { get; } = new NameEqualityComparer();

        public static IEqualityComparer<TopicEntity> EntityComparer { get; } = new TopicEntityEqualityComparer();

        public string EntityName { get; }
        public bool Durable { get; }
        public bool AutoDelete { get; }
        public IDictionary<string, object> TopicAttributes { get; }
        public IDictionary<string, object> TopicSubscriptionAttributes { get; }
        public IDictionary<string, string> TopicTags { get; }
        public long Id { get; }
        public Topic Topic => this;

        public override string ToString()
        {
            return string.Join(", ",
                new[]
                {
                    $"name: {EntityName}",
                    Durable ? "durable" : "",
                    AutoDelete ? "auto-delete" : "",
                    TopicTags.Any() ? $"tags: {string.Join(";", TopicTags.Select(a => $"{a.Key}={a.Value}"))}" : "",
                    TopicAttributes.Any() ? $"attributes: {string.Join(";", TopicAttributes.Select(a => $"{a.Key}={a.Value}"))}" : "",
                    TopicSubscriptionAttributes.Any()
                        ? $"subscription-attributes: {string.Join(";", TopicSubscriptionAttributes.Select(a => $"{a.Key}={a.Value}"))}"
                        : ""
                }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        void EnsureRawDeliveryIsSet()
        {
            TopicSubscriptionAttributes["RawMessageDelivery"] = "true";
        }


        sealed class NameEqualityComparer : IEqualityComparer<TopicEntity>
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

                return string.Equals(x.EntityName, y.EntityName);
            }

            public int GetHashCode(TopicEntity obj)
            {
                return obj.EntityName.GetHashCode();
            }
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

                return string.Equals(x.EntityName, y.EntityName) && x.Durable == y.Durable && x.AutoDelete == y.AutoDelete;
            }

            public int GetHashCode(TopicEntity obj)
            {
                unchecked
                {
                    var hashCode = obj.EntityName.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.Durable.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.AutoDelete.GetHashCode();
                    return hashCode;
                }
            }
        }
    }
}
