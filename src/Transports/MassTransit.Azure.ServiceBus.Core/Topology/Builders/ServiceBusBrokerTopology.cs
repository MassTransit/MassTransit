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
                exchangeScope.Set(topic.TopicDescription);
            }

            foreach (var queue in Queues)
            {
                var exchangeScope = context.CreateScope("queue");
                exchangeScope.Set(queue.QueueDescription);
            }

            foreach (var subscription in Subscriptions)
            {
                var subscriptionScope = context.CreateScope("subscription");
                subscriptionScope.Set(subscription.SubscriptionDescription);
            }

            foreach (var queueSubscription in QueueSubscriptions)
            {
                var queueSubscriptionScope = context.CreateScope("queueSubscription");
                queueSubscriptionScope.Set(queueSubscription.Subscription.SubscriptionDescription);
            }

            foreach (var topicSubscription in TopicSubscriptions)
            {
                var topicSubscriptionScope = context.CreateScope("topicSubscription");
                topicSubscriptionScope.Set(topicSubscription.Subscription.SubscriptionDescription);
            }
        }
    }
}
