namespace MassTransit.ActiveMqTransport.Topology
{
    public class PublishEndpointBrokerTopologyBuilder :
        BrokerTopologyBuilder,
        IPublishEndpointBrokerTopologyBuilder
    {
        readonly PublishBrokerTopologyOptions _options;

        public PublishEndpointBrokerTopologyBuilder(PublishBrokerTopologyOptions options = PublishBrokerTopologyOptions.FlattenHierarchy)
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
            return new ActiveMqBrokerTopology(Topics, Queues, Consumers);
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

            public TopicHandle CreateTopic(string name, bool durable, bool autoDelete)
            {
                return _builder.CreateTopic(name, durable, autoDelete);
            }

            public QueueHandle CreateQueue(string name, bool autoDelete)
            {
                return _builder.CreateQueue(name, autoDelete);
            }

            public ConsumerHandle BindConsumer(TopicHandle topic, QueueHandle queue, string selector)
            {
                return _builder.BindConsumer(topic, queue, selector);
            }
        }
    }
}
