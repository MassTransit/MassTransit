namespace MassTransit.AzureServiceBusTransport.Topology
{
    using System.Collections.Generic;
    using System.Linq;
    using Azure.Messaging.ServiceBus.Administration;


    public class QueueSubscriptionEntity :
        QueueSubscription,
        QueueSubscriptionHandle
    {
        readonly QueueEntity _queue;
        readonly SubscriptionEntity _subscription;
        readonly TopicEntity _topic;

        public QueueSubscriptionEntity(long id, long subscriptionId, TopicEntity topic, QueueEntity queue, CreateSubscriptionOptions createSubscriptionOptions,
            CreateRuleOptions rule = null, RuleFilter filter = null)
        {
            Id = id;

            _topic = topic;
            _queue = queue;
            _subscription = new SubscriptionEntity(subscriptionId, topic, createSubscriptionOptions, rule, filter);
        }

        public static IEqualityComparer<QueueSubscriptionEntity> EntityComparer { get; } = new QueueSubscriptionEntityEqualityComparer();
        public static IEqualityComparer<QueueSubscriptionEntity> NameComparer { get; } = new NameEqualityComparer();

        public Topic Source => _topic.Topic;
        public Queue Destination => _queue.Queue;
        public Subscription Subscription => _subscription;

        public long Id { get; }
        public QueueSubscription QueueSubscription => this;

        public override string ToString()
        {
            return string.Join(", ",
                new[]
                {
                    $"topic: {_topic.CreateTopicOptions.Name}",
                    $"queue: {_queue.CreateQueueOptions.Name}",
                    $"subscription: {_subscription.CreateSubscriptionOptions.SubscriptionName}"
                }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }


        sealed class QueueSubscriptionEntityEqualityComparer :
            IEqualityComparer<QueueSubscriptionEntity>
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

                return TopicEntity.EntityComparer.Equals(x._topic, y._topic)
                    && QueueEntity.EntityComparer.Equals(x._queue, y._queue)
                    && SubscriptionEntity.EntityComparer.Equals(x._subscription, y._subscription);
            }

            public int GetHashCode(QueueSubscriptionEntity obj)
            {
                unchecked
                {
                    var hashCode = TopicEntity.EntityComparer.GetHashCode(obj._topic);
                    hashCode = (hashCode * 397) ^ QueueEntity.EntityComparer.GetHashCode(obj._queue);
                    hashCode = (hashCode * 397) ^ SubscriptionEntity.EntityComparer.GetHashCode(obj._subscription);

                    return hashCode;
                }
            }
        }


        sealed class NameEqualityComparer :
            IEqualityComparer<QueueSubscriptionEntity>
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

                return string.Equals(x.Subscription.CreateSubscriptionOptions.SubscriptionName, y.Subscription.CreateSubscriptionOptions.SubscriptionName)
                    && string.Equals(x.Subscription.CreateSubscriptionOptions.TopicName, y.Subscription.CreateSubscriptionOptions.TopicName)
                    && string.Equals(x.Destination.CreateQueueOptions.Name, y.Destination.CreateQueueOptions.Name);
            }

            public int GetHashCode(QueueSubscriptionEntity obj)
            {
                var hashCode = obj.Subscription.CreateSubscriptionOptions.SubscriptionName.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Subscription.CreateSubscriptionOptions.TopicName.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Destination.CreateQueueOptions.Name.GetHashCode();

                return hashCode;
            }
        }
    }
}
