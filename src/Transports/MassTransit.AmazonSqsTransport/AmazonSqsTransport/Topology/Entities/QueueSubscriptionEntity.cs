namespace MassTransit.AmazonSqsTransport.Topology
{
    using System.Collections.Generic;
    using System.Linq;


    public class QueueSubscriptionEntity :
        QueueSubscription,
        QueueSubscriptionHandle
    {
        readonly QueueEntity _queue;
        readonly TopicEntity _topic;

        public QueueSubscriptionEntity(long id, TopicEntity topic, QueueEntity queue)
        {
            Id = id;
            _topic = topic;
            _queue = queue;
        }

        public static IEqualityComparer<QueueSubscriptionEntity> NameComparer { get; } = new NameEqualityComparer();
        public static IEqualityComparer<QueueSubscriptionEntity> EntityComparer { get; } = new ConsumerEntityEqualityComparer();

        public Topic Source => _topic.Topic;
        public Queue Destination => _queue.Queue;

        public long Id { get; }
        public QueueSubscription QueueSubscription => this;

        public override string ToString()
        {
            return string.Join(", ",
                new[] {$"source: {Source.EntityName}", $"destination: {Destination.EntityName}"}.Where(x => !string.IsNullOrWhiteSpace(x)));
        }


        sealed class ConsumerEntityEqualityComparer : IEqualityComparer<QueueSubscriptionEntity>
        {
            public bool Equals(QueueSubscriptionEntity x, QueueSubscriptionEntity y)
            {
                if (ReferenceEquals(x, y))
                    return true;

                if (ReferenceEquals(x, null))
                    return false;

                if (ReferenceEquals(y, null))
                    return false;

                if (x.GetType() != y.GetType())
                    return false;

                return x._topic.Equals(y._topic) && x._queue.Equals(y._queue);
            }

            public int GetHashCode(QueueSubscriptionEntity obj)
            {
                unchecked
                {
                    var hashCode = obj._topic.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj._queue.GetHashCode();

                    return hashCode;
                }
            }
        }


        sealed class NameEqualityComparer : IEqualityComparer<QueueSubscriptionEntity>
        {
            public bool Equals(QueueSubscriptionEntity x, QueueSubscriptionEntity y)
            {
                if (ReferenceEquals(x, y))
                    return true;

                if (ReferenceEquals(x, null))
                    return false;

                if (ReferenceEquals(y, null))
                    return false;

                if (x.GetType() != y.GetType())
                    return false;

                return string.Equals(x._topic.EntityName, y._topic.EntityName);
            }

            public int GetHashCode(QueueSubscriptionEntity obj)
            {
                return obj._topic.EntityName.GetHashCode();
            }
        }
    }
}
