namespace MassTransit.ActiveMqTransport.Topology
{
    using System.Collections.Generic;
    using System.Linq;


    public class ActiveMqBrokerTopology :
        BrokerTopology
    {
        public ActiveMqBrokerTopology(IEnumerable<Topic> topics, IEnumerable<Queue> queues, IEnumerable<Consumer> consumers)
        {
            Topics = topics.ToArray();
            Queues = queues.ToArray();
            Consumers = consumers.ToArray();
        }

        public Topic[] Topics { get; }
        public Queue[] Queues { get; }
        public Consumer[] Consumers { get; }

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
                    queue.AutoDelete
                });
            }

            foreach (var binding in Consumers)
            {
                var consumerScope = context.CreateScope("consumer");
                consumerScope.Set(new
                {
                    Source = binding.Source.EntityName,
                    Destination = binding.Destination.EntityName,
                    RoutingKey = binding.Selector
                });
            }
        }
    }
}
