namespace MassTransit.AzureServiceBusTransport.Topology
{
    using System.Collections.Generic;
    using System.Linq;
    using Azure.Messaging.ServiceBus.Administration;


    public class SubscriptionEntity :
        Subscription,
        SubscriptionHandle
    {
        readonly TopicEntity _topic;

        public SubscriptionEntity(long id, TopicEntity topic, CreateSubscriptionOptions createSubscriptionOptions, CreateRuleOptions rule = null,
            RuleFilter filter = null)
        {
            Id = id;

            _topic = topic;

            CreateSubscriptionOptions = createSubscriptionOptions;

            Rule = rule;
            Filter = filter;
        }

        public static IEqualityComparer<SubscriptionEntity> NameComparer { get; } = new NameEqualityComparer();
        public static IEqualityComparer<SubscriptionEntity> EntityComparer { get; } = new SubscriptionEntityEqualityComparer();

        public CreateSubscriptionOptions CreateSubscriptionOptions { get; }

        public TopicHandle Topic => _topic;

        public CreateRuleOptions Rule { get; }
        public RuleFilter Filter { get; }
        public long Id { get; }
        public Subscription Subscription => this;

        public override string ToString()
        {
            return string.Join(", ",
                new[] { $"topic: {_topic.CreateTopicOptions.Name}", $"subscription: {CreateSubscriptionOptions.SubscriptionName}" }.Where(x =>
                    !string.IsNullOrWhiteSpace(x)));
        }


        sealed class SubscriptionEntityEqualityComparer :
            IEqualityComparer<SubscriptionEntity>
        {
            public bool Equals(SubscriptionEntity x, SubscriptionEntity y)
            {
                if (ReferenceEquals(x, y))
                    return true;

                if (ReferenceEquals(x, null))
                    return false;

                if (ReferenceEquals(y, null))
                    return false;

                if (x.GetType() != y.GetType())
                    return false;

                return string.Equals(x.CreateSubscriptionOptions.SubscriptionName, y.CreateSubscriptionOptions.SubscriptionName)
                    && string.Equals(x.CreateSubscriptionOptions.TopicName, y.CreateSubscriptionOptions.TopicName)
                    && x.CreateSubscriptionOptions.AutoDeleteOnIdle == y.CreateSubscriptionOptions.AutoDeleteOnIdle
                    && x.CreateSubscriptionOptions.DefaultMessageTimeToLive == y.CreateSubscriptionOptions.DefaultMessageTimeToLive
                    && x.CreateSubscriptionOptions.EnableBatchedOperations == y.CreateSubscriptionOptions.EnableBatchedOperations
                    && x.CreateSubscriptionOptions.DeadLetteringOnMessageExpiration == y.CreateSubscriptionOptions.DeadLetteringOnMessageExpiration
                    && x.CreateSubscriptionOptions.EnableDeadLetteringOnFilterEvaluationExceptions
                    == y.CreateSubscriptionOptions.EnableDeadLetteringOnFilterEvaluationExceptions
                    && string.Equals(x.CreateSubscriptionOptions.ForwardDeadLetteredMessagesTo, y.CreateSubscriptionOptions.ForwardDeadLetteredMessagesTo)
                    && string.Equals(x.CreateSubscriptionOptions.ForwardTo, y.CreateSubscriptionOptions.ForwardTo)
                    && x.CreateSubscriptionOptions.LockDuration == y.CreateSubscriptionOptions.LockDuration
                    && x.CreateSubscriptionOptions.MaxDeliveryCount == y.CreateSubscriptionOptions.MaxDeliveryCount
                    && x.CreateSubscriptionOptions.RequiresSession == y.CreateSubscriptionOptions.RequiresSession
                    && string.Equals(x.CreateSubscriptionOptions.UserMetadata, y.CreateSubscriptionOptions.UserMetadata);
            }

            public int GetHashCode(SubscriptionEntity obj)
            {
                unchecked
                {
                    var hashCode = obj.CreateSubscriptionOptions.SubscriptionName.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.CreateSubscriptionOptions.TopicName.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.CreateSubscriptionOptions.AutoDeleteOnIdle.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.CreateSubscriptionOptions.DefaultMessageTimeToLive.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.CreateSubscriptionOptions.EnableBatchedOperations.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.CreateSubscriptionOptions.DeadLetteringOnMessageExpiration.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.CreateSubscriptionOptions.EnableDeadLetteringOnFilterEvaluationExceptions.GetHashCode();
                    if (!string.IsNullOrWhiteSpace(obj.CreateSubscriptionOptions.ForwardDeadLetteredMessagesTo))
                        hashCode = (hashCode * 397) ^ obj.CreateSubscriptionOptions.ForwardDeadLetteredMessagesTo.GetHashCode();

                    if (!string.IsNullOrWhiteSpace(obj.CreateSubscriptionOptions.ForwardTo))
                        hashCode = (hashCode * 397) ^ obj.CreateSubscriptionOptions.ForwardTo.GetHashCode();

                    hashCode = (hashCode * 397) ^ obj.CreateSubscriptionOptions.LockDuration.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.CreateSubscriptionOptions.MaxDeliveryCount.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.CreateSubscriptionOptions.RequiresSession.GetHashCode();
                    if (!string.IsNullOrWhiteSpace(obj.CreateSubscriptionOptions.UserMetadata))
                        hashCode = (hashCode * 397) ^ obj.CreateSubscriptionOptions.UserMetadata.GetHashCode();

                    return hashCode;
                }
            }
        }


        sealed class NameEqualityComparer :
            IEqualityComparer<SubscriptionEntity>
        {
            public bool Equals(SubscriptionEntity x, SubscriptionEntity y)
            {
                if (ReferenceEquals(x, y))
                    return true;

                if (ReferenceEquals(x, null))
                    return false;

                if (ReferenceEquals(y, null))
                    return false;

                if (x.GetType() != y.GetType())
                    return false;

                return string.Equals(x.CreateSubscriptionOptions.SubscriptionName, y.CreateSubscriptionOptions.SubscriptionName)
                    && string.Equals(x.CreateSubscriptionOptions.TopicName, y.CreateSubscriptionOptions.TopicName);
            }

            public int GetHashCode(SubscriptionEntity obj)
            {
                var hashCode = obj.CreateSubscriptionOptions.SubscriptionName.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.CreateSubscriptionOptions.TopicName.GetHashCode();

                return hashCode;
            }
        }
    }
}
