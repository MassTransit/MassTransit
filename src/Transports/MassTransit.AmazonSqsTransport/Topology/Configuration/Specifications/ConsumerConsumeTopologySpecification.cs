namespace MassTransit.AmazonSqsTransport.Topology.Specifications
{
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using Configurators;
    using Entities;
    using GreenPipes;


    /// <summary>
    /// Used to by a TopicSubscription destination to the receive endpoint, via an additional message consumer
    /// </summary>
    public class ConsumerConsumeTopologySpecification :
        TopicSubscriptionConfigurator,
        IAmazonSqsConsumeTopologySpecification
    {
        public ConsumerConsumeTopologySpecification(string topicName, bool durable = true, bool autoDelete = false)
            : base(topicName, durable, autoDelete)
        {
        }

        public ConsumerConsumeTopologySpecification(Topic topic)
            : base(topic)
        {
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return Enumerable.Empty<ValidationResult>();
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            var topicHandle = builder.CreateTopic(EntityName, Durable, AutoDelete, TopicAttributes, TopicSubscriptionAttributes, Tags);

            var topicSubscriptionHandle = builder.CreateQueueSubscription(topicHandle, builder.Queue);
        }
    }
}
