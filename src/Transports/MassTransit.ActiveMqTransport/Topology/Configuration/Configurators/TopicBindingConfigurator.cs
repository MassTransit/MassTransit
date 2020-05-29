namespace MassTransit.ActiveMqTransport.Topology.Configurators
{
    using Entities;


    public class TopicBindingConfigurator :
        TopicConfigurator,
        ITopicBindingConfigurator
    {
        public TopicBindingConfigurator(string topicName, bool durable = true, bool autoDelete = false, string selector = null)
            : base(topicName, durable, autoDelete)
        {
            Selector = selector;
        }

        public TopicBindingConfigurator(Topic topic, string selector = null)
            : base(topic)
        {
            Selector = selector;
        }

        public string Selector { get; set; }
    }
}
