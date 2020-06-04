namespace MassTransit.Azure.ServiceBus.Core.Topology.Builders
{
    using System;
    using System.Linq;
    using Entities;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Management;


    public class PublishEndpointBrokerTopologyBuilder :
        BrokerTopologyBuilder,
        IPublishEndpointBrokerTopologyBuilder
    {
        [Flags]
        public enum Options
        {
            FlattenHierarchy = 0,
            MaintainHierarchy = 1
        }


        readonly IServiceBusPublishTopology _topology;
        readonly Options _options;

        public PublishEndpointBrokerTopologyBuilder(IServiceBusPublishTopology topology, Options options = Options.MaintainHierarchy)
        {
            _topology = topology;
            _options = options;
        }

        /// <summary>
        /// The topic where the published message is sent
        /// </summary>
        public TopicHandle Topic { get; set; }

        public IPublishEndpointBrokerTopologyBuilder CreateImplementedBuilder()
        {
            if (_options.HasFlag(Options.MaintainHierarchy))
                return new ImplementedBuilder(this, _topology, _options);

            return this;
        }


        class ImplementedBuilder :
            IPublishEndpointBrokerTopologyBuilder
        {
            readonly IPublishEndpointBrokerTopologyBuilder _builder;
            readonly IServiceBusPublishTopology _topology;
            readonly Options _options;
            TopicHandle _topic;

            public ImplementedBuilder(IPublishEndpointBrokerTopologyBuilder builder, IServiceBusPublishTopology topology, Options options)
            {
                _builder = builder;
                _topology = topology;
                _options = options;
            }

            public TopicHandle Topic
            {
                get => _topic;
                set
                {
                    _topic = value;
                    if (_builder.Topic != null)
                    {
                        var subscriptionName = string.Join("-", value.Topic.TopicDescription.Path.Split('/').Reverse());
                        var subscriptionDescription = new SubscriptionDescription(_builder.Topic.Topic.TopicDescription.Path,
                            _topology.FormatSubscriptionName(subscriptionName)) {ForwardTo = value.Topic.TopicDescription.Path};

                        _builder.CreateTopicSubscription(_builder.Topic, _topic, subscriptionDescription);
                    }
                }
            }

            public IPublishEndpointBrokerTopologyBuilder CreateImplementedBuilder()
            {
                return _options.HasFlag(Options.MaintainHierarchy) ? new ImplementedBuilder(this, _topology, _options) : this;
            }

            public TopicHandle CreateTopic(TopicDescription topicDescription)
            {
                return _builder.CreateTopic(topicDescription);
            }

            public SubscriptionHandle CreateSubscription(TopicHandle topic, SubscriptionDescription subscriptionDescription, RuleDescription rule,
                Filter filter)
            {
                return _builder.CreateSubscription(topic, subscriptionDescription, rule, filter);
            }

            public TopicSubscriptionHandle CreateTopicSubscription(TopicHandle source, TopicHandle destination, SubscriptionDescription subscriptionDescription)
            {
                return _builder.CreateTopicSubscription(source, destination, subscriptionDescription);
            }

            public QueueHandle CreateQueue(QueueDescription queueDescription)
            {
                return _builder.CreateQueue(queueDescription);
            }

            public QueueSubscriptionHandle CreateQueueSubscription(TopicHandle exchange, QueueHandle queue, SubscriptionDescription subscriptionDescription,
                RuleDescription rule,
                Filter filter)
            {
                return _builder.CreateQueueSubscription(exchange, queue, subscriptionDescription, rule, filter);
            }
        }
    }
}
