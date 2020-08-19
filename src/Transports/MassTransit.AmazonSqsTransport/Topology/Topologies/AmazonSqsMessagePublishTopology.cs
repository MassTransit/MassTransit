namespace MassTransit.AmazonSqsTransport.Topology.Topologies
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


    public class AmazonSqsMessagePublishTopology<TMessage> :
        MessagePublishTopology<TMessage>,
        IAmazonSqsMessagePublishTopologyConfigurator<TMessage>
        where TMessage : class
    {
        readonly IMessageTopology<TMessage> _messageTopology;
        readonly TopicConfigurator _topic;

        public AmazonSqsMessagePublishTopology(IMessageTopology<TMessage> messageTopology)
        {
            _messageTopology = messageTopology;

            var topicName = _messageTopology.EntityName;

            var temporary = TypeMetadataCache<TMessage>.IsTemporaryMessageType;

            var durable = !temporary;
            var autoDelete = temporary;

            _topic = new TopicConfigurator(topicName, durable, autoDelete);
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
            var topicHandle = builder.CreateTopic(_topic.EntityName, _topic.Durable, _topic.AutoDelete, _topic.TopicAttributes,
                _topic.TopicSubscriptionAttributes, _topic.Tags);

            builder.Topic ??= topicHandle;
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
    }
}
