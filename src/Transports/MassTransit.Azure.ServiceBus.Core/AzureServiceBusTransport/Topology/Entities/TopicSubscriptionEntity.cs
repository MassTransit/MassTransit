namespace MassTransit.AzureServiceBusTransport.Topology
{
    using System.Collections.Generic;
    using System.Linq;
    using Azure.Messaging.ServiceBus.Administration;


    public class TopicSubscriptionEntity :
        TopicSubscription,
        TopicSubscriptionHandle
    {
        readonly TopicEntity _destination;
        readonly TopicEntity _source;
        readonly SubscriptionEntity _subscription;

        public TopicSubscriptionEntity(long id, long subscriptionId, TopicEntity source, TopicEntity destination,
            CreateSubscriptionOptions createSubscriptionOptions, CreateRuleOptions rule = null, RuleFilter filter = null)
        {
            Id = id;
            _source = source;
            _destination = destination;
            _subscription = new SubscriptionEntity(subscriptionId, source, createSubscriptionOptions, rule, filter);
        }

        public static IEqualityComparer<TopicSubscriptionEntity> EntityComparer { get; } = new TopicSubscriptionEntityEqualityComparer();
        public static IEqualityComparer<TopicSubscriptionEntity> NameComparer { get; } = new NameEqualityComparer();

        public Topic Source => _source.Topic;
        public Topic Destination => _destination.Topic;
        public Subscription Subscription => _subscription.Subscription;

        public long Id { get; }
        public TopicSubscription TopicSubscription => this;

        public override string ToString()
        {
            return string.Join(", ",
                new[]
                {
                    $"source: {_source.CreateTopicOptions.Name}",
                    $"destination: {_destination.CreateTopicOptions.Name}",
                    $"subscription: {_subscription.CreateSubscriptionOptions.SubscriptionName}"
                }.Where(x => !string.IsNullOrWhiteSpace(x)));
        }


        sealed class TopicSubscriptionEntityEqualityComparer :
            IEqualityComparer<TopicSubscriptionEntity>
        {
            public bool Equals(TopicSubscriptionEntity x, TopicSubscriptionEntity y)
            {
                if (ReferenceEquals(x, y))
                    return true;

                if (ReferenceEquals(x, null))
                    return false;

                if (ReferenceEquals(y, null))
                    return false;

                if (x.GetType() != y.GetType())
                    return false;

                return TopicEntity.EntityComparer.Equals(x._source, y._source)
                    && TopicEntity.EntityComparer.Equals(x._destination, y._destination)
                    && SubscriptionEntity.EntityComparer.Equals(x._subscription, y._subscription);
            }

            public int GetHashCode(TopicSubscriptionEntity obj)
            {
                unchecked
                {
                    var hashCode = TopicEntity.EntityComparer.GetHashCode(obj._source);
                    hashCode = (hashCode * 397) ^ TopicEntity.EntityComparer.GetHashCode(obj._destination);
                    hashCode = (hashCode * 397) ^ SubscriptionEntity.EntityComparer.GetHashCode(obj._subscription);

                    return hashCode;
                }
            }
        }


        sealed class NameEqualityComparer :
            IEqualityComparer<TopicSubscriptionEntity>
        {
            public bool Equals(TopicSubscriptionEntity x, TopicSubscriptionEntity y)
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
                    && string.Equals(x.Destination.CreateTopicOptions.Name, y.Destination.CreateTopicOptions.Name);
            }

            public int GetHashCode(TopicSubscriptionEntity obj)
            {
                var hashCode = obj.Subscription.CreateSubscriptionOptions.SubscriptionName.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Subscription.CreateSubscriptionOptions.TopicName.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Destination.CreateTopicOptions.Name.GetHashCode();

                return hashCode;
            }
        }
    }
}
