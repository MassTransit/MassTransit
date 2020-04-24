namespace MassTransit.AmazonSqsTransport.Topology.Topologies
{
    using System;
    using System.Collections.Generic;
    using AmazonSqsTransport.Configuration;
    using Builders;
    using Configurators;
    using Entities;
    using MassTransit.Topology;
    using MassTransit.Topology.Topologies;
    using Metadata;
    using Settings;


    public class AmazonSqsMessagePublishTopology<TMessage> :
        MessagePublishTopology<TMessage>,
        IAmazonSqsMessagePublishTopologyConfigurator<TMessage>
        where TMessage : class
    {
        readonly TopicConfigurator _topic;
        readonly IList<IAmazonSqsMessagePublishTopology> _implementedMessageTypes;
        readonly IMessageTopology<TMessage> _messageTopology;

        public AmazonSqsMessagePublishTopology(IMessageTopology<TMessage> messageTopology)
        {
            _messageTopology = messageTopology;

            var topicName = _messageTopology.EntityName;

            var temporary = TypeMetadataCache<TMessage>.IsTemporaryMessageType;

            var durable = !temporary;
            var autoDelete = temporary;

            _topic = new TopicConfigurator(topicName, durable, autoDelete);

            _implementedMessageTypes = new List<IAmazonSqsMessagePublishTopology>();
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

        IDictionary<string, object> ITopicConfigurator.TopicAttributes => _topic.TopicAttributes;
        IDictionary<string, object> ITopicConfigurator.TopicSubscriptionAttributes => _topic.TopicSubscriptionAttributes;
        IDictionary<string, string> ITopicConfigurator.TopicTags => _topic.TopicTags;

        public AmazonSqsEndpointAddress GetEndpointAddress(Uri hostAddress)
        {
            return _topic.GetEndpointAddress(hostAddress);
        }

        public override bool TryGetPublishAddress(Uri baseAddress, out Uri publishAddress)
        {
            publishAddress = _topic.GetEndpointAddress(baseAddress);
            return true;
        }

        public void Apply(IPublishEndpointBrokerTopologyBuilder builder)
        {
            var topicHandle = builder.CreateTopic(_topic.EntityName, _topic.Durable, _topic.AutoDelete, _topic.TopicAttributes, _topic
                .TopicSubscriptionAttributes, _topic.Tags);

            builder.Topic ??= topicHandle;

            foreach (IAmazonSqsMessagePublishTopology configurator in _implementedMessageTypes)
                configurator.Apply(builder);
        }

        public PublishSettings GetPublishSettings(Uri hostAddress)
        {
            return new TopicPublishSettings(GetEndpointAddress(hostAddress));
        }

        public BrokerTopology GetBrokerTopology(PublishBrokerTopologyOptions options)
        {
            var builder = new PublishEndpointBrokerTopologyBuilder(options);

            Apply(builder);

            return builder.BuildBrokerTopology();
        }

        public void AddImplementedMessageConfigurator<T>(IAmazonSqsMessagePublishTopologyConfigurator<T> configurator, bool direct)
            where T : class
        {
            var adapter = new ImplementedTypeAdapter<T>(configurator, direct);

            _implementedMessageTypes.Add(adapter);
        }


        class ImplementedTypeAdapter<T> :
            IAmazonSqsMessagePublishTopology
            where T : class
        {
            readonly IAmazonSqsMessagePublishTopologyConfigurator<T> _configurator;
            readonly bool _direct;

            public ImplementedTypeAdapter(IAmazonSqsMessagePublishTopologyConfigurator<T> configurator, bool direct)
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
