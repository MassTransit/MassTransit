namespace MassTransit.ActiveMqTransport.Topology.Topologies
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using Configurators;
    using Entities;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Metadata;
    using Settings;


    public class ActiveMqMessagePublishTopology<TMessage> :
        MessagePublishTopology<TMessage>,
        IActiveMqMessagePublishTopologyConfigurator<TMessage>
        where TMessage : class
    {
        readonly IList<IActiveMqMessagePublishTopology> _implementedMessageTypes;
        readonly IMessageTopology<TMessage> _messageTopology;
        readonly TopicConfigurator _topic;

        public ActiveMqMessagePublishTopology(IMessageTopology<TMessage> messageTopology)
        {
            _messageTopology = messageTopology;

            var topicName = $"VirtualTopic.{messageTopology.EntityName}";

            var temporary = TypeMetadataCache<TMessage>.IsTemporaryMessageType;

            var durable = !temporary;
            var autoDelete = temporary;

            _topic = new TopicConfigurator(topicName, durable, autoDelete);

            _implementedMessageTypes = new List<IActiveMqMessagePublishTopology>();
        }

        public Topic Topic => _topic;

        bool ITopicConfigurator.Durable
        {
            set => _topic.Durable = value;
        }

        bool ITopicConfigurator.AutoDelete
        {
            set => _topic.AutoDelete = value;
        }

        public override bool TryGetPublishAddress(Uri baseAddress, out Uri publishAddress)
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
            return new TopicSendSettings(_topic.GetEndpointAddress(hostAddress));
        }

        public BrokerTopology GetBrokerTopology(PublishBrokerTopologyOptions options)
        {
            var builder = new PublishEndpointBrokerTopologyBuilder(options);

            Apply(builder);

            return builder.BuildBrokerTopology();
        }

        public void AddImplementedMessageConfigurator<T>(IActiveMqMessagePublishTopologyConfigurator<T> configurator, bool direct)
            where T : class
        {
            var adapter = new ImplementedTypeAdapter<T>(configurator, direct);

            _implementedMessageTypes.Add(adapter);
        }


        class ImplementedTypeAdapter<T> :
            IActiveMqMessagePublishTopology
            where T : class
        {
            readonly IActiveMqMessagePublishTopologyConfigurator<T> _configurator;
            readonly bool _direct;

            public ImplementedTypeAdapter(IActiveMqMessagePublishTopologyConfigurator<T> configurator, bool direct)
            {
                _configurator = configurator;
                _direct = direct;
            }

            public void Apply(IPublishEndpointBrokerTopologyBuilder builder)
            {
                if (_direct)
                {
                    var implementedBuilder = builder.CreateImplementedBuilder();

                    _configurator.Apply(implementedBuilder);
                }
            }
        }
    }
}
