namespace MassTransit.Azure.ServiceBus.Core.Topology.Entities
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Management;


    public class SubscriptionEntity :
        Subscription,
        SubscriptionHandle
    {
        readonly TopicEntity _topic;

        public SubscriptionEntity(long id, TopicEntity topic, SubscriptionDescription subscriptionDescription, RuleDescription rule = null,
            Filter filter = null)
        {
            Id = id;

            _topic = topic;

            SubscriptionDescription = subscriptionDescription;

            Rule = rule;
            Filter = filter;
        }

        public static IEqualityComparer<SubscriptionEntity> NameComparer { get; } = new NameEqualityComparer();
        public static IEqualityComparer<SubscriptionEntity> EntityComparer { get; } = new SubscriptionEntityEqualityComparer();

        public TopicHandle Topic => _topic;

        public RuleDescription Rule { get; }
        public Filter Filter { get; }

        public SubscriptionDescription SubscriptionDescription { get; }
        public long Id { get; }
        public Subscription Subscription => this;

        public override string ToString()
        {
            return string.Join(", ",
                new[] {$"topic: {_topic.TopicDescription.Path}", $"subscription: {SubscriptionDescription.SubscriptionName}"}.Where(x =>
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

                return string.Equals(x.SubscriptionDescription.SubscriptionName, y.SubscriptionDescription.SubscriptionName)
                    && string.Equals(x.SubscriptionDescription.TopicPath, y.SubscriptionDescription.TopicPath)
                    && x.SubscriptionDescription.AutoDeleteOnIdle == y.SubscriptionDescription.AutoDeleteOnIdle
                    && x.SubscriptionDescription.DefaultMessageTimeToLive == y.SubscriptionDescription.DefaultMessageTimeToLive
                    && x.SubscriptionDescription.EnableBatchedOperations == y.SubscriptionDescription.EnableBatchedOperations
                    && x.SubscriptionDescription.EnableDeadLetteringOnMessageExpiration == y.SubscriptionDescription.EnableDeadLetteringOnMessageExpiration
                    && x.SubscriptionDescription.EnableDeadLetteringOnFilterEvaluationExceptions
                    == y.SubscriptionDescription.EnableDeadLetteringOnFilterEvaluationExceptions
                    && string.Equals(x.SubscriptionDescription.ForwardDeadLetteredMessagesTo, y.SubscriptionDescription.ForwardDeadLetteredMessagesTo)
                    && string.Equals(x.SubscriptionDescription.ForwardTo, y.SubscriptionDescription.ForwardTo)
                    && x.SubscriptionDescription.LockDuration == y.SubscriptionDescription.LockDuration
                    && x.SubscriptionDescription.MaxDeliveryCount == y.SubscriptionDescription.MaxDeliveryCount
                    && x.SubscriptionDescription.RequiresSession == y.SubscriptionDescription.RequiresSession
                    && string.Equals(x.SubscriptionDescription.UserMetadata, y.SubscriptionDescription.UserMetadata);
            }

            public int GetHashCode(SubscriptionEntity obj)
            {
                unchecked
                {
                    var hashCode = obj.SubscriptionDescription.SubscriptionName.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.SubscriptionDescription.TopicPath.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.SubscriptionDescription.AutoDeleteOnIdle.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.SubscriptionDescription.DefaultMessageTimeToLive.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.SubscriptionDescription.EnableBatchedOperations.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.SubscriptionDescription.EnableDeadLetteringOnMessageExpiration.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.SubscriptionDescription.EnableDeadLetteringOnFilterEvaluationExceptions.GetHashCode();
                    if (!string.IsNullOrWhiteSpace(obj.SubscriptionDescription.ForwardDeadLetteredMessagesTo))
                        hashCode = (hashCode * 397) ^ obj.SubscriptionDescription.ForwardDeadLetteredMessagesTo.GetHashCode();

                    if (!string.IsNullOrWhiteSpace(obj.SubscriptionDescription.ForwardTo))
                        hashCode = (hashCode * 397) ^ obj.SubscriptionDescription.ForwardTo.GetHashCode();

                    hashCode = (hashCode * 397) ^ obj.SubscriptionDescription.LockDuration.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.SubscriptionDescription.MaxDeliveryCount.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.SubscriptionDescription.RequiresSession.GetHashCode();
                    if (!string.IsNullOrWhiteSpace(obj.SubscriptionDescription.UserMetadata))
                        hashCode = (hashCode * 397) ^ obj.SubscriptionDescription.UserMetadata.GetHashCode();

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

                return string.Equals(x.SubscriptionDescription.SubscriptionName, y.SubscriptionDescription.SubscriptionName)
                    && string.Equals(x.SubscriptionDescription.TopicPath, y.SubscriptionDescription.TopicPath);
            }

            public int GetHashCode(SubscriptionEntity obj)
            {
                var hashCode = obj.SubscriptionDescription.SubscriptionName.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.SubscriptionDescription.TopicPath.GetHashCode();

                return hashCode;
            }
        }
    }
}
