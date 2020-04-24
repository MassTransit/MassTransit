namespace MassTransit.AmazonSqsTransport.Topology.Configurators
{
    using Configuration;
    using Entities;


    public class TopicSubscriptionConfigurator :
        TopicConfigurator,
        ITopicSubscriptionConfigurator
    {
        public TopicSubscriptionConfigurator(string topicName, bool durable = true, bool autoDelete = false)
            : base(topicName, durable, autoDelete)
        {
        }

        public TopicSubscriptionConfigurator(Topic topic)
            : base(topic)
        {
        }
    }
}
