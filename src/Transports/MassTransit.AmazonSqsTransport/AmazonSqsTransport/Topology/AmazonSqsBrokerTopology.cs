namespace MassTransit.AmazonSqsTransport.Topology
{
    using System.Collections.Generic;
    using System.Linq;


    public class AmazonSqsBrokerTopology :
        BrokerTopology
    {
        public AmazonSqsBrokerTopology(IEnumerable<Topic> exchanges, IEnumerable<Queue> queues, IEnumerable<QueueSubscription> queueSubscriptions,
            IEnumerable<TopicSubscription> topicSubscriptions)
        {
            Topics = exchanges.ToArray();
            Queues = queues.ToArray();
            QueueSubscriptions = queueSubscriptions.ToArray();
            TopicSubscriptions = topicSubscriptions.ToArray();
        }

        public Topic[] Topics { get; }
        public Queue[] Queues { get; }
        public QueueSubscription[] QueueSubscriptions { get; }
        public TopicSubscription[] TopicSubscriptions { get; }

        void IProbeSite.Probe(ProbeContext context)
        {
            foreach (var topic in Topics)
            {
                var topicScope = context.CreateScope("topic");
                topicScope.Set(new
                {
                    Name = topic.EntityName,
                    topic.Durable,
                    topic.AutoDelete
                });
            }

            foreach (var queue in Queues)
            {
                var queueScope = context.CreateScope("queue");
                queueScope.Set(new
                {
                    Name = queue.EntityName,
                    queue.Durable,
                    queue.AutoDelete
                });
            }

            foreach (var subscription in QueueSubscriptions)
            {
                var subscriptionScope = context.CreateScope("queueSubscription");
                subscriptionScope.Set(new
                {
                    Source = subscription.Source.EntityName,
                    Destination = subscription.Destination.EntityName
                });
            }

            foreach (var subscription in TopicSubscriptions)
            {
                var subscriptionScope = context.CreateScope("topicSubscription");
                subscriptionScope.Set(new
                {
                    Source = subscription.Source.EntityName,
                    Destination = subscription.Destination.EntityName
                });
            }
        }
    }
}
