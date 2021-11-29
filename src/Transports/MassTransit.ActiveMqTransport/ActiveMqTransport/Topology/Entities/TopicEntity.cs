namespace MassTransit.ActiveMqTransport.Topology
{
    using System.Collections.Generic;
    using System.Linq;


    public class TopicEntity :
        Topic,
        TopicHandle
    {
        public TopicEntity(long id, string name, bool durable, bool autoDelete)
        {
            Id = id;
            EntityName = name;
            Durable = durable;
            AutoDelete = autoDelete;
        }

        public static IEqualityComparer<TopicEntity> NameComparer { get; } = new NameEqualityComparer();

        public static IEqualityComparer<TopicEntity> EntityComparer { get; } = new ExchangeEntityEqualityComparer();

        public string EntityName { get; }
        public bool Durable { get; }
        public bool AutoDelete { get; }
        public long Id { get; }
        public Topic Topic => this;

        public override string ToString()
        {
            return string.Join(", ",
                new[] {$"name: {EntityName}", Durable ? "durable" : "", AutoDelete ? "auto-delete" : ""}.Where(x => !string.IsNullOrWhiteSpace(x)));
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


        sealed class ExchangeEntityEqualityComparer :
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
