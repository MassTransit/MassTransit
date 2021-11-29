namespace MassTransit.ActiveMqTransport.Configuration
{
    using Topology;


    public class ActiveMqTopicConfigurator :
        EntityConfigurator,
        IActiveMqTopicConfigurator,
        Topic
    {
        public ActiveMqTopicConfigurator(string topicName, bool durable = true, bool autoDelete = false)
            : base(topicName, durable, autoDelete)
        {
        }

        public ActiveMqTopicConfigurator(Topic source)
            : base(source.EntityName, source.Durable, source.AutoDelete)
        {
        }

        protected override ActiveMqEndpointAddress.AddressType AddressType => ActiveMqEndpointAddress.AddressType.Topic;
    }
}
