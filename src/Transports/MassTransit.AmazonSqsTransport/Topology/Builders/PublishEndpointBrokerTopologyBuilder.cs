namespace MassTransit.AmazonSqsTransport.Topology.Builders
{
    using System.Collections.Generic;
    using Entities;


    public class PublishEndpointBrokerTopologyBuilder :
        BrokerTopologyBuilder,
        IPublishEndpointBrokerTopologyBuilder
    {
        readonly PublishBrokerTopologyOptions _options;

        public PublishEndpointBrokerTopologyBuilder(PublishBrokerTopologyOptions options = PublishBrokerTopologyOptions.MaintainHierarchy)
        {
            _options = options;
        }

        /// <summary>
        /// The exchange to which the published message is sent
        /// </summary>
        public TopicHandle Topic { get; set; }

        public IPublishEndpointBrokerTopologyBuilder CreateImplementedBuilder()
        {
            if (_options.HasFlag(PublishBrokerTopologyOptions.MaintainHierarchy))
                return new ImplementedBuilder(this, _options);

            return this;
        }

        public BrokerTopology BuildBrokerTopology()
        {
            return new AmazonSqsBrokerTopology(Topics, Queues, QueueSubscriptions, TopicSubscriptions);
        }


        class ImplementedBuilder :
            IPublishEndpointBrokerTopologyBuilder
        {
            readonly IPublishEndpointBrokerTopologyBuilder _builder;
            readonly PublishBrokerTopologyOptions _options;

            public ImplementedBuilder(IPublishEndpointBrokerTopologyBuilder builder, PublishBrokerTopologyOptions options)
            {
                _builder = builder;
                _options = options;
            }

            public TopicHandle Topic { get; set; }

            public IPublishEndpointBrokerTopologyBuilder CreateImplementedBuilder()
            {
                if (_options.HasFlag(PublishBrokerTopologyOptions.MaintainHierarchy))
                    return new ImplementedBuilder(this, _options);

                return this;
            }

            public TopicHandle CreateTopic(string name, bool durable, bool autoDelete, IDictionary<string, object> topicAttributes = null,
                IDictionary<string, object> topicSubscriptionAttributes = null, IDictionary<string, string> tags = null)
            {
                return _builder.CreateTopic(name, durable, autoDelete, topicAttributes, topicSubscriptionAttributes, tags);
            }

            public QueueHandle CreateQueue(string name, bool durable, bool autoDelete, IDictionary<string, object> queueAttributes = null,
                IDictionary<string, object> queueSubscriptionAttributes = null, IDictionary<string, string> tags = null)
            {
                return _builder.CreateQueue(name, durable, autoDelete, queueAttributes, queueSubscriptionAttributes, tags);
            }

            public QueueSubscriptionHandle CreateQueueSubscription(TopicHandle topic, QueueHandle queue)
            {
                return _builder.CreateQueueSubscription(topic, queue);
            }

            public TopicSubscriptionHandle CreateTopicSubscription(TopicHandle source, TopicHandle destination)
            {
                return _builder.CreateTopicSubscription(source, destination);
            }
        }
    }
}
