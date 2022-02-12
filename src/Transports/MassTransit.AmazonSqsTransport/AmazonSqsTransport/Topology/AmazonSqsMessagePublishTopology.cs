#nullable enable
namespace MassTransit.AmazonSqsTransport.Topology
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Internals;
    using MassTransit.Topology;


    public class AmazonSqsMessagePublishTopology<TMessage> :
        MessagePublishTopology<TMessage>,
        IAmazonSqsMessagePublishTopologyConfigurator<TMessage>
        where TMessage : class
    {
        readonly AmazonSqsTopicConfigurator _amazonSqsTopic;
        readonly IAmazonSqsPublishTopology _publishTopology;

        public AmazonSqsMessagePublishTopology(IAmazonSqsPublishTopology publishTopology, IMessageTopology<TMessage> messageTopology)
        {
            _publishTopology = publishTopology;

            var topicName = messageTopology.EntityName;

            var temporary = MessageTypeCache<TMessage>.IsTemporaryMessageType;

            var durable = !temporary;
            var autoDelete = temporary;

            _amazonSqsTopic = new AmazonSqsTopicConfigurator(topicName, durable, autoDelete);
        }

        public Topic Topic => _amazonSqsTopic;

        bool IAmazonSqsTopicConfigurator.Durable
        {
            set => _amazonSqsTopic.Durable = value;
        }

        bool IAmazonSqsTopicConfigurator.AutoDelete
        {
            set => _amazonSqsTopic.AutoDelete = value;
        }

        IDictionary<string, object> IAmazonSqsTopicConfigurator.TopicAttributes => _amazonSqsTopic.TopicAttributes;
        IDictionary<string, object> IAmazonSqsTopicConfigurator.TopicSubscriptionAttributes => _amazonSqsTopic.TopicSubscriptionAttributes;
        IDictionary<string, string> IAmazonSqsTopicConfigurator.TopicTags => _amazonSqsTopic.TopicTags;

        public AmazonSqsEndpointAddress GetEndpointAddress(Uri hostAddress)
        {
            return _amazonSqsTopic.GetEndpointAddress(hostAddress);
        }

        public override bool TryGetPublishAddress(Uri baseAddress, out Uri? publishAddress)
        {
            publishAddress = _amazonSqsTopic.GetEndpointAddress(baseAddress);
            return true;
        }

        public void Apply(IPublishEndpointBrokerTopologyBuilder builder)
        {
            var topicHandle = builder.CreateTopic(_amazonSqsTopic.EntityName, _amazonSqsTopic.Durable, _amazonSqsTopic.AutoDelete,
                _publishTopology.TopicAttributes.MergeLeft(_amazonSqsTopic.TopicAttributes),
                _publishTopology.TopicSubscriptionAttributes.MergeLeft(_amazonSqsTopic.TopicSubscriptionAttributes),
                _publishTopology.TopicTags.MergeLeft(_amazonSqsTopic.Tags));

            builder.Topic ??= topicHandle;
        }

        public PublishSettings GetPublishSettings(Uri hostAddress)
        {
            return new TopicPublishSettings(GetEndpointAddress(hostAddress));
        }

        public BrokerTopology GetBrokerTopology()
        {
            var builder = new PublishEndpointBrokerTopologyBuilder();

            Apply(builder);

            return builder.BuildBrokerTopology();
        }
    }
}
