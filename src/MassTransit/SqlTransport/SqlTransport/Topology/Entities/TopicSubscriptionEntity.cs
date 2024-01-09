#nullable enable
namespace MassTransit.SqlTransport.Topology
{
    using System.Collections.Generic;
    using System.Linq;


    public class TopicSubscriptionEntity :
        TopicToTopicSubscription,
        TopicSubscriptionHandle
    {
        readonly TopicEntity _destination;
        readonly TopicEntity _source;

        public TopicSubscriptionEntity(long id, TopicEntity source, TopicEntity destination, SqlSubscriptionType subscriptionType, string? routingKey)
        {
            Id = id;
            SubscriptionType = subscriptionType;
            RoutingKey = routingKey;
            _source = source;
            _destination = destination;
        }

        public static IEqualityComparer<TopicSubscriptionEntity> EntityComparer { get; } = new TopicSubscriptionEntityEqualityComparer();
        public long Id { get; }
        public TopicToTopicSubscription Subscription => this;
        public SqlSubscriptionType SubscriptionType { get; }

        public Topic Source => _source.Topic;
        public Topic Destination => _destination.Topic;
        public string? RoutingKey { get; }

        public override string ToString()
        {
            return string.Join(", ",
                new[]
                {
                    $"source: {Source.TopicName}",
                    $"destination: {Destination.TopicName}",
                    $"type: {SubscriptionType}",
                    string.IsNullOrWhiteSpace(RoutingKey) ? "" : $"routing-key: {RoutingKey}"
                }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }


        sealed class TopicSubscriptionEntityEqualityComparer : IEqualityComparer<TopicSubscriptionEntity>
        {
            public bool Equals(TopicSubscriptionEntity? x, TopicSubscriptionEntity? y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null))
                    return false;
                if (ReferenceEquals(y, null))
                    return false;
                if (x.GetType() != y.GetType())
                    return false;
                return x._source.Equals(y._source) && x.SubscriptionType == y.SubscriptionType && x.RoutingKey == y.RoutingKey;
            }

            public int GetHashCode(TopicSubscriptionEntity obj)
            {
                unchecked
                {
                    var hashCode = obj._source.GetHashCode();
                    hashCode = (hashCode * 397) ^ (int)obj.SubscriptionType;
                    hashCode = (hashCode * 397) ^ (obj.RoutingKey != null ? obj.RoutingKey.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }
    }
}
