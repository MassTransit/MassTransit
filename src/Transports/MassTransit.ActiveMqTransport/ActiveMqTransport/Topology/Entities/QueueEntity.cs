namespace MassTransit.ActiveMqTransport.Topology
{
    using System.Collections.Generic;
    using System.Linq;


    public class QueueEntity :
        Queue,
        QueueHandle
    {
        public QueueEntity(long id, string name, bool durable, bool autoDelete)
        {
            Id = id;
            EntityName = name;
            AutoDelete = autoDelete;
            Durable = durable;
        }

        public static IEqualityComparer<QueueEntity> NameComparer { get; } = new NameEqualityComparer();

        public static IEqualityComparer<QueueEntity> QueueComparer { get; } = new QueueEntityEqualityComparer();

        public string EntityName { get; }
        public bool AutoDelete { get; }
        public bool Durable { get; }
        public long Id { get; }
        public Queue Queue => this;

        public override string ToString()
        {
            return string.Join(", ",
                new[] { $"name: {EntityName}", AutoDelete ? "auto-delete" : "" }.Where(x => !string.IsNullOrWhiteSpace(x)));
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
                return string.Equals(x.EntityName, y.EntityName) && x.AutoDelete == y.AutoDelete && x.Durable == y.Durable;
            }

            public int GetHashCode(QueueEntity obj)
            {
                unchecked
                {
                    var hashCode = obj.EntityName.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.AutoDelete.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.Durable.GetHashCode(); //TODO
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
