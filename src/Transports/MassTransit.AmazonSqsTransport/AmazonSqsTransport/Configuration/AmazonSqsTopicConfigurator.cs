namespace MassTransit.AmazonSqsTransport.Configuration
{
    using System.Collections.Generic;
    using Topology;


    public class AmazonSqsTopicConfigurator :
        EntityConfigurator,
        IAmazonSqsTopicConfigurator,
        Topic
    {
        public AmazonSqsTopicConfigurator(string topicName, bool durable = true, bool autoDelete = false, IDictionary<string, object> topicAttributes = null,
            IDictionary<string, object> topicSubscriptionAttributes = null, IDictionary<string, string> topicTags = null)
            : base(topicName, durable, autoDelete)
        {
            TopicAttributes = topicAttributes ?? new Dictionary<string, object>();
            TopicSubscriptionAttributes = topicSubscriptionAttributes ?? new Dictionary<string, object>();
            TopicTags = topicTags ?? new Dictionary<string, string>();

            if (AmazonSqsEndpointAddress.IsFifo(topicName))
                TopicAttributes["FifoTopic"] = "true";
        }

        public AmazonSqsTopicConfigurator(Topic source)
            : this(source.EntityName, source.Durable, source.AutoDelete, source.TopicAttributes, source.TopicSubscriptionAttributes, source.TopicTags)
        {
        }

        public IDictionary<string, string> Tags => TopicTags;

        protected override AmazonSqsEndpointAddress.AddressType AddressType => AmazonSqsEndpointAddress.AddressType.Topic;

        public IDictionary<string, object> TopicAttributes { get; private set; }
        public IDictionary<string, object> TopicSubscriptionAttributes { get; private set; }
        public IDictionary<string, string> TopicTags { get; private set; }
    }
}
