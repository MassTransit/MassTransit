namespace MassTransit.ActiveMqTransport.Topology.Specifications
{
    using System.Collections.Generic;
    using System.Linq;
    using Builders;
    using Configurators;
    using Entities;
    using GreenPipes;


    /// <summary>
    /// Used to by a Consumer virtual destination to the receive endpoint, via an additional message consumer
    /// </summary>
    public class ConsumerConsumeTopologySpecification :
        TopicBindingConfigurator,
        IActiveMqConsumeTopologySpecification
    {
        readonly string _consumerName;

        public ConsumerConsumeTopologySpecification(string topicName, string consumerName, bool durable = true, bool autoDelete = false)
            : base(topicName, durable, autoDelete)
        {
            _consumerName = consumerName;
        }

        public ConsumerConsumeTopologySpecification(Topic topic, string consumerName)
            : base(topic)
        {
            _consumerName = consumerName;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return Enumerable.Empty<ValidationResult>();
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            var topic = builder.CreateTopic(EntityName, Durable, AutoDelete);

            var destinationQueue = builder.Queue.Queue;

            var consumerQueueName = _consumerName.Replace("{queue}", destinationQueue.EntityName);

            var queue = builder.CreateQueue(consumerQueueName, destinationQueue.Durable, destinationQueue.AutoDelete);

            var consumer = builder.BindConsumer(topic, queue, Selector);
        }
    }
}
