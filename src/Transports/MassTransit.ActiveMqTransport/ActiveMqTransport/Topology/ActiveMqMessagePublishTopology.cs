#nullable enable
namespace MassTransit.ActiveMqTransport.Topology
{
    using System;
    using Configuration;
    using MassTransit.Topology;


    public class ActiveMqMessagePublishTopology<TMessage> :
        MessagePublishTopology<TMessage>,
        IActiveMqMessagePublishTopologyConfigurator<TMessage>
        where TMessage : class
    {
        readonly ActiveMqTopicConfigurator _topic;

        public ActiveMqMessagePublishTopology(IActiveMqPublishTopology publishTopology, IMessageTopology<TMessage> messageTopology)
        {
            var topicName = $"{publishTopology.VirtualTopicPrefix}{messageTopology.EntityName}";

            var temporary = MessageTypeCache<TMessage>.IsTemporaryMessageType;

            var durable = !temporary;
            var autoDelete = temporary;

            _topic = new ActiveMqTopicConfigurator(topicName, durable, autoDelete);
        }

        public Topic Topic => _topic;

        bool IActiveMqTopicConfigurator.Durable
        {
            set => _topic.Durable = value;
        }

        bool IActiveMqTopicConfigurator.AutoDelete
        {
            set => _topic.AutoDelete = value;
        }

        public override bool TryGetPublishAddress(Uri baseAddress, out Uri? publishAddress)
        {
            publishAddress = _topic.GetEndpointAddress(baseAddress);
            return true;
        }

        public void Apply(IPublishEndpointBrokerTopologyBuilder builder)
        {
            builder.Topic = builder.CreateTopic(_topic.EntityName, _topic.Durable, _topic.AutoDelete);

            // this was disabled previously, so not sure if it can be added
            // foreach (IActiveMqMessagePublishTopology configurator in _implementedMessageTypes)
            //     configurator.Apply(builder);
        }

        public SendSettings GetSendSettings(Uri hostAddress)
        {
            return new ActiveMqTopicSendSettings(_topic.GetEndpointAddress(hostAddress));
        }

        public BrokerTopology GetBrokerTopology(PublishBrokerTopologyOptions options)
        {
            var builder = new PublishEndpointBrokerTopologyBuilder(options);

            Apply(builder);

            return builder.BuildBrokerTopology();
        }
    }
}
