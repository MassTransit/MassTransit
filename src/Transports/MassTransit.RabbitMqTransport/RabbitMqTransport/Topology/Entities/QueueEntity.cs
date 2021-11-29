namespace MassTransit.RabbitMqTransport.Topology
{
    using System.Collections.Generic;
    using System.Linq;


    public class QueueEntity :
        Queue,
        QueueHandle
    {
        public QueueEntity(long id, string name, bool durable, bool autoDelete, bool exclusive, IDictionary<string, object> arguments)
        {
            Id = id;
            QueueName = name;
            Durable = durable;
            AutoDelete = autoDelete;
            Exclusive = exclusive;
            QueueArguments = arguments ?? new Dictionary<string, object>();
        }

        public static IEqualityComparer<QueueEntity> NameComparer { get; } = new NameEqualityComparer();

        public static IEqualityComparer<QueueEntity> QueueComparer { get; } = new QueueEntityEqualityComparer();

        public string QueueName { get; }
        public bool Durable { get; }
        public bool AutoDelete { get; }
        public bool Exclusive { get; }
        public IDictionary<string, object> QueueArguments { get; }
        public long Id { get; }
        public Queue Queue => this;

        public override string ToString()
        {
            return string.Join(", ",
                new[]
                {
                    $"name: {QueueName}",
                    Durable ? "durable" : "",
                    AutoDelete ? "auto-delete" : "",
                    Exclusive ? "exclusive" : "",
                    string.Join(", ", QueueArguments.Select(x => $"{x.Key}: {x.Value}"))
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
                return string.Equals(x.QueueName, y.QueueName) && x.Durable == y.Durable && x.AutoDelete == y.AutoDelete && x.Exclusive == y.Exclusive
                    && x.QueueArguments.All(a => y.QueueArguments.TryGetValue(a.Key, out var value) && a.Value.Equals(value));
            }

            public int GetHashCode(QueueEntity obj)
            {
                unchecked
                {
                    var hashCode = obj.QueueName.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.Durable.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.AutoDelete.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.Exclusive.GetHashCode();
                    foreach (KeyValuePair<string, object> keyValuePair in obj.QueueArguments)
                    {
                        hashCode = (hashCode * 397) ^ keyValuePair.Key.GetHashCode();
                        hashCode = (hashCode * 397) ^ keyValuePair.Value.GetHashCode();
                    }

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
                return string.Equals(x.QueueName, y.QueueName);
            }

            public int GetHashCode(QueueEntity obj)
            {
                return obj.QueueName.GetHashCode();
            }
        }
    }
}
