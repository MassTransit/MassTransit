namespace MassTransit.ActiveMqTransport.Topology.Configurators
{
    using Entities;


    public class TopicConfigurator :
        EntityConfigurator,
        ITopicConfigurator,
        Topic
    {
        public TopicConfigurator(string topicName, bool durable = true, bool autoDelete = false)
            : base(topicName, durable, autoDelete)
        {
        }

        public TopicConfigurator(Topic source)
            : base(source.EntityName, source.Durable, source.AutoDelete)
        {
        }

        protected override ActiveMqEndpointAddress.AddressType AddressType => ActiveMqEndpointAddress.AddressType.Topic;
    }
}
