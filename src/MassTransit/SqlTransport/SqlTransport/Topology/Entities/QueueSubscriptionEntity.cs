#nullable enable
namespace MassTransit.SqlTransport.Topology
{
    using System.Collections.Generic;
    using System.Linq;


    public class QueueSubscriptionEntity :
        TopicToQueueSubscription,
        QueueSubscriptionHandle
    {
        readonly QueueEntity _queue;
        readonly TopicEntity _topic;

        public QueueSubscriptionEntity(long id, TopicEntity topic, QueueEntity queue, SqlSubscriptionType subscriptionType, string? routingKey)
        {
            Id = id;
            SubscriptionType = subscriptionType;
            RoutingKey = routingKey;
            _topic = topic;
            _queue = queue;
        }

        public static IEqualityComparer<QueueSubscriptionEntity> EntityComparer { get; } = new QueueSubscriptionEntityEqualityComparer();

        public long Id { get; }
        public TopicToQueueSubscription Subscription => this;
        public SqlSubscriptionType SubscriptionType { get; }

        public Topic Source => _topic.Topic;
        public Queue Destination => _queue.Queue;
        public string? RoutingKey { get; }

        public override string ToString()
        {
            return string.Join(", ",
                new[]
                {
                    $"source: {Source.TopicName}",
                    $"destination: {Destination.QueueName}",
                    $"type: {SubscriptionType}",
                    string.IsNullOrWhiteSpace(RoutingKey) ? "" : $"routing-key: {RoutingKey}",
                }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }


        sealed class QueueSubscriptionEntityEqualityComparer : IEqualityComparer<QueueSubscriptionEntity>
        {
            public bool Equals(QueueSubscriptionEntity? x, QueueSubscriptionEntity? y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null))
                    return false;
                if (ReferenceEquals(y, null))
                    return false;
                if (x.GetType() != y.GetType())
                    return false;
                return x._queue.Equals(y._queue) && x._topic.Equals(y._topic) && x.SubscriptionType == y.SubscriptionType && x.RoutingKey == y.RoutingKey;
            }

            public int GetHashCode(QueueSubscriptionEntity obj)
            {
                unchecked
                {
                    var hashCode = obj._queue.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj._topic.GetHashCode();
                    hashCode = (hashCode * 397) ^ (int)obj.SubscriptionType;
                    hashCode = (hashCode * 397) ^ (obj.RoutingKey != null ? obj.RoutingKey.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }
    }
}
