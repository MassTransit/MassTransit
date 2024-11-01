using MassTransit.ActiveMqTransport.Topology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MassTransit.ActiveMqTransport.Configuration
{
    public class ConsumerConsumeTopicTopologySpecification :
        ActiveMqTopicBindingConfigurator,
        IActiveMqConsumeTopologySpecification
    {
        readonly IActiveMqConsumerEndpointQueueNameFormatter _consumerEndpointQueueNameFormatter;

        public ConsumerConsumeTopicTopologySpecification(string topicName, IActiveMqConsumerEndpointQueueNameFormatter consumerEndpointQueueNameFormatter,
            bool durable = true, bool autoDelete = false)
            : base(topicName, durable, autoDelete)
        {
            _consumerEndpointQueueNameFormatter = consumerEndpointQueueNameFormatter;
        }

        public ConsumerConsumeTopicTopologySpecification(Topic topic, IActiveMqConsumerEndpointQueueNameFormatter consumerEndpointQueueNameFormatter)
            : base(topic)
        {
            _consumerEndpointQueueNameFormatter = consumerEndpointQueueNameFormatter;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return Enumerable.Empty<ValidationResult>();
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            var topic = builder.CreateTopic(EntityName, Durable, AutoDelete);

            _ = builder.BindConsumer(topic, null, Selector);
        }
    }
}
