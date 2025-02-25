#nullable enable
namespace MassTransit.SqlTransport.Topology
{
    using System;


    public class PublishEndpointBrokerTopologyBuilder :
        BrokerTopologyBuilder,
        IPublishEndpointBrokerTopologyBuilder
    {
        /// <summary>
        /// The topic to which the published message is sent
        /// </summary>
        public TopicHandle? Topic { get; set; }

        public IPublishEndpointBrokerTopologyBuilder CreateImplementedBuilder()
        {
            return new ImplementedBuilder(this);
        }


        class ImplementedBuilder :
            IPublishEndpointBrokerTopologyBuilder
        {
            readonly IPublishEndpointBrokerTopologyBuilder _builder;
            TopicHandle? _topic;

            public ImplementedBuilder(IPublishEndpointBrokerTopologyBuilder builder)
            {
                _builder = builder;
            }

            public TopicHandle? Topic
            {
                get => _topic;
                set
                {
                    _topic = value;
                    if (_builder.Topic != null && _topic != null)
                        _builder.CreateTopicSubscription(_builder.Topic, _topic);
                }
            }

            public IPublishEndpointBrokerTopologyBuilder CreateImplementedBuilder()
            {
                return new ImplementedBuilder(this);
            }

            public TopicHandle CreateTopic(string name)
            {
                return _builder.CreateTopic(name);
            }

            public TopicSubscriptionHandle CreateTopicSubscription(TopicHandle source, TopicHandle destination, SqlSubscriptionType subscriptionType,
                string? routingKey)
            {
                return _builder.CreateTopicSubscription(source, destination, subscriptionType, routingKey);
            }

            public QueueHandle CreateQueue(string name, TimeSpan? autoDeleteOnIdle, int? maxDeliveryCount = null)
            {
                return _builder.CreateQueue(name, autoDeleteOnIdle, maxDeliveryCount);
            }

            public QueueSubscriptionHandle CreateQueueSubscription(TopicHandle topic, QueueHandle queue, SqlSubscriptionType subscriptionType,
                string? routingKey)
            {
                return _builder.CreateQueueSubscription(topic, queue, subscriptionType, routingKey);
            }
        }
    }
}
