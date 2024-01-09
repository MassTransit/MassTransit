#nullable enable
namespace MassTransit.SqlTransport.Topology
{
    using System.Collections.Generic;


    public class TopicEntity :
        Topic,
        TopicHandle
    {
        public TopicEntity(long id, string name)
        {
            Id = id;
            TopicName = name;
        }

        public static IEqualityComparer<TopicEntity> NameComparer { get; } = new NameEqualityComparer();
        public static IEqualityComparer<TopicEntity> EntityComparer { get; } = new NameEqualityComparer();

        public string TopicName { get; }
        public long Id { get; }
        public Topic Topic => this;

        public override string ToString()
        {
            return TopicName;
        }


        sealed class NameEqualityComparer : IEqualityComparer<TopicEntity>
        {
            public bool Equals(TopicEntity? x, TopicEntity? y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null))
                    return false;
                if (ReferenceEquals(y, null))
                    return false;
                if (x.GetType() != y.GetType())
                    return false;
                return string.Equals(x.TopicName, y.TopicName);
            }

            public int GetHashCode(TopicEntity obj)
            {
                return obj.TopicName.GetHashCode();
            }
        }
    }
}
