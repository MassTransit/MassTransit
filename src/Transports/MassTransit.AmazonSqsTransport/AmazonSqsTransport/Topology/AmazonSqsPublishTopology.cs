namespace MassTransit.AmazonSqsTransport.Topology
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Topology;


    public class AmazonSqsPublishTopology :
        PublishTopology,
        IAmazonSqsPublishTopologyConfigurator
    {
        readonly IMessageTopology _messageTopology;

        public AmazonSqsPublishTopology(IMessageTopology messageTopology)
        {
            _messageTopology = messageTopology;

            TopicAttributes = new Dictionary<string, object>();
            TopicSubscriptionAttributes = new Dictionary<string, object>();
            TopicTags = new Dictionary<string, string>();
        }

        public IDictionary<string, object> TopicAttributes { get; private set; }
        public IDictionary<string, object> TopicSubscriptionAttributes { get; private set; }
        public IDictionary<string, string> TopicTags { get; private set; }

        IAmazonSqsMessagePublishTopology<T> IAmazonSqsPublishTopology.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IAmazonSqsMessagePublishTopology<T>;
        }

        IAmazonSqsMessagePublishTopologyConfigurator<T> IAmazonSqsPublishTopologyConfigurator.GetMessageTopology<T>()
        {
            return GetMessageTopology<T>() as IAmazonSqsMessagePublishTopologyConfigurator<T>;
        }

        protected override IMessagePublishTopologyConfigurator CreateMessageTopology<T>(Type type)
        {
            var messageTopology = new AmazonSqsMessagePublishTopology<T>(this, _messageTopology.GetMessageTopology<T>());

            OnMessageTopologyCreated(messageTopology);

            return messageTopology;
        }
    }
}
