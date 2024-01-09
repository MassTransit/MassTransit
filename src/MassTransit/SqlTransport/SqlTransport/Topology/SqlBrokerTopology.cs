namespace MassTransit.SqlTransport.Topology
{
    using System.Collections.Generic;
    using System.Linq;


    public class SqlBrokerTopology :
        BrokerTopology
    {
        public SqlBrokerTopology(IEnumerable<Topic> topics, IEnumerable<TopicToTopicSubscription> topicSubscriptions, IEnumerable<Queue> queues,
            IEnumerable<TopicToQueueSubscription> queueSubscriptions)
        {
            Topics = topics.ToArray();
            Queues = queues.ToArray();
            TopicSubscriptions = topicSubscriptions.ToArray();
            QueueSubscriptions = queueSubscriptions.ToArray();
        }

        public Topic[] Topics { get; }
        public Queue[] Queues { get; }
        public TopicToTopicSubscription[] TopicSubscriptions { get; }
        public TopicToQueueSubscription[] QueueSubscriptions { get; }

        public void Probe(ProbeContext context)
        {
            foreach (var topic in Topics)
            {
                var scope = context.CreateScope("topic");
                scope.Set(new { Name = topic.TopicName });
            }

            foreach (var queue in Queues)
            {
                var scope = context.CreateScope("queue");
                scope.Set(new
                {
                    Name = queue.QueueName,
                    queue.AutoDeleteOnIdle
                });
            }

            foreach (var subscription in TopicSubscriptions)
            {
                var scope = context.CreateScope("topic-subscription");
                scope.Set(new
                {
                    Source = subscription.Source.TopicName,
                    Destination = subscription.Destination.TopicName,
                    subscription.SubscriptionType,
                    subscription.RoutingKey
                });
            }

            foreach (var subscription in QueueSubscriptions)
            {
                var scope = context.CreateScope("queue-subscription");
                scope.Set(new
                {
                    Source = subscription.Source.TopicName,
                    Destination = subscription.Destination.QueueName,
                    subscription.SubscriptionType,
                    subscription.RoutingKey
                });
            }
        }
    }
}
