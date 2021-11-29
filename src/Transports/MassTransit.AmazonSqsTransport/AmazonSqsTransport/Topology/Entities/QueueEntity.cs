namespace MassTransit.AmazonSqsTransport.Topology
{
    using System.Collections.Generic;
    using System.Linq;


    public class QueueEntity :
        Queue,
        QueueHandle
    {
        public QueueEntity(long id, string name, bool durable, bool autoDelete, IDictionary<string, object> queueAttributes = null,
            IDictionary<string, object> queueSubscriptionAttributes = null, IDictionary<string, string> queueTags = null)
        {
            Id = id;
            EntityName = name;
            Durable = durable;
            AutoDelete = autoDelete;
            QueueAttributes = queueAttributes ?? new Dictionary<string, object>();
            QueueSubscriptionAttributes = queueSubscriptionAttributes ?? new Dictionary<string, object>();
            QueueTags = queueTags ?? new Dictionary<string, string>();
        }

        public static IEqualityComparer<QueueEntity> NameComparer { get; } = new NameEqualityComparer();

        public static IEqualityComparer<QueueEntity> QueueComparer { get; } = new QueueEntityEqualityComparer();

        public string EntityName { get; }
        public bool Durable { get; }
        public bool AutoDelete { get; }
        public IDictionary<string, object> QueueAttributes { get; }
        public IDictionary<string, object> QueueSubscriptionAttributes { get; }
        public IDictionary<string, string> QueueTags { get; }
        public long Id { get; }
        public Queue Queue => this;

        public override string ToString()
        {
            return string.Join(", ",
                new[]
                {
                    $"name: {EntityName}",
                    Durable ? "durable" : "",
                    AutoDelete ? "auto-delete" : "",
                    QueueTags.Any() ? $"tags: {string.Join(";", QueueTags.Select(a => $"{a.Key}={a.Value}"))}" : "",
                    QueueAttributes.Any() ? $"attributes: {string.Join(";", QueueAttributes.Select(a => $"{a.Key}={a.Value}"))}" : "",
                    QueueSubscriptionAttributes.Any()
                        ? $"subscription-attributes: {string.Join(";", QueueSubscriptionAttributes.Select(a => $"{a.Key}={a.Value}"))}"
                        : ""
                }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }


        sealed class QueueEntityEqualityComparer : IEqualityComparer<QueueEntity>
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
                return string.Equals(x.EntityName, y.EntityName) && x.Durable == y.Durable && x.AutoDelete == y.AutoDelete;
            }

            public int GetHashCode(QueueEntity obj)
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


        sealed class NameEqualityComparer : IEqualityComparer<QueueEntity>
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
                return string.Equals(x.EntityName, y.EntityName);
            }

            public int GetHashCode(QueueEntity obj)
            {
                return obj.EntityName.GetHashCode();
            }
        }
    }
}
