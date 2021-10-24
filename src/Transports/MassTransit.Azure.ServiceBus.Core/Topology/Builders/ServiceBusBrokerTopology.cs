namespace MassTransit.Azure.ServiceBus.Core.Topology.Builders
{
    using System.Collections.Generic;
    using System.Linq;
    using Entities;
    using GreenPipes;


    public class ServiceBusBrokerTopology :
        BrokerTopology
    {
        public ServiceBusBrokerTopology(IEnumerable<Topic> topics, IEnumerable<Subscription> subscriptions, IEnumerable<Queue> queues,
            IEnumerable<QueueSubscription> queueSubscriptions, IEnumerable<TopicSubscription> topicSubscriptions)
        {
            Topics = topics.ToArray();
            Queues = queues.ToArray();
            Subscriptions = subscriptions.ToArray();
            QueueSubscriptions = queueSubscriptions.ToArray();
            TopicSubscriptions = topicSubscriptions.ToArray();
        }

        public Topic[] Topics { get; }
        public Queue[] Queues { get; }
        public Subscription[] Subscriptions { get; }
        public QueueSubscription[] QueueSubscriptions { get; }
        public TopicSubscription[] TopicSubscriptions { get; }

        void IProbeSite.Probe(ProbeContext context)
        {
            foreach (var topic in Topics)
            {
                var exchangeScope = context.CreateScope("topic");
                exchangeScope.Set(new
                {
                    topic.TopicDescription.Name,
                    topic.TopicDescription.EnablePartitioning,
                    topic.TopicDescription.SupportOrdering,
                    topic.TopicDescription.EnableBatchedOperations,
                    topic.TopicDescription.RequiresDuplicateDetection,
                    topic.TopicDescription.AutoDeleteOnIdle,
                    topic.TopicDescription.DefaultMessageTimeToLive,
                    topic.TopicDescription.DuplicateDetectionHistoryTimeWindow,
                    topic.TopicDescription.MaxSizeInMegabytes
                });
            }

            foreach (var queue in Queues)
            {
                var exchangeScope = context.CreateScope("queue");
                exchangeScope.Set(new
                {
                    queue.QueueDescription.Name,
                    queue.QueueDescription.EnablePartitioning,
                    queue.QueueDescription.EnableBatchedOperations,
                    queue.QueueDescription.ForwardTo,
                    queue.QueueDescription.LockDuration,
                    queue.QueueDescription.MaxDeliveryCount,
                    queue.QueueDescription.RequiresSession,
                    queue.QueueDescription.RequiresDuplicateDetection,
                    queue.QueueDescription.AutoDeleteOnIdle,
                    queue.QueueDescription.DefaultMessageTimeToLive,
                    queue.QueueDescription.DuplicateDetectionHistoryTimeWindow,
                    queue.QueueDescription.MaxSizeInMegabytes,
                    queue.QueueDescription.ForwardDeadLetteredMessagesTo,
                    queue.QueueDescription.DeadLetteringOnMessageExpiration
                });
            }

            foreach (var subscription in Subscriptions)
            {
                var subscriptionScope = context.CreateScope("subscription");
                subscriptionScope.Set(GetSubscriptionValues(subscription));
            }

            foreach (var queueSubscription in QueueSubscriptions)
            {
                var queueSubscriptionScope = context.CreateScope("queueSubscription");
                queueSubscriptionScope.Set(GetSubscriptionValues(queueSubscription.Subscription));
            }

            foreach (var topicSubscription in TopicSubscriptions)
            {
                var topicSubscriptionScope = context.CreateScope("topicSubscription");
                topicSubscriptionScope.Set(GetSubscriptionValues(topicSubscription.Subscription));
            }
        }

        static object GetSubscriptionValues(Subscription subscription)
        {
            return new
            {
                subscription.SubscriptionDescription.SubscriptionName,
                subscription.SubscriptionDescription.EnableBatchedOperations,
                subscription.SubscriptionDescription.ForwardTo,
                subscription.SubscriptionDescription.LockDuration,
                subscription.SubscriptionDescription.MaxDeliveryCount,
                subscription.SubscriptionDescription.RequiresSession,
                subscription.SubscriptionDescription.AutoDeleteOnIdle,
                subscription.SubscriptionDescription.DefaultMessageTimeToLive,
                subscription.SubscriptionDescription.ForwardDeadLetteredMessagesTo,
                subscription.SubscriptionDescription.DeadLetteringOnMessageExpiration
            };
        }
    }
}
