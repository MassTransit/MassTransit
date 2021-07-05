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
        readonly IActiveMqConsumerEndpointQueueNameFormatter _consumerEndpointQueueNameFormatter;

        public ConsumerConsumeTopologySpecification(string topicName, IActiveMqConsumerEndpointQueueNameFormatter consumerEndpointQueueNameFormatter,
            bool durable = true, bool autoDelete = false)
            : base(topicName, durable, autoDelete)
        {
            _consumerEndpointQueueNameFormatter = consumerEndpointQueueNameFormatter;
        }

        public ConsumerConsumeTopologySpecification(Topic topic, IActiveMqConsumerEndpointQueueNameFormatter consumerEndpointQueueNameFormatter)
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
            var destinationQueue = builder.Queue.Queue;

            var topicName = EntityName;

            var consumerEndpointQueueName = _consumerEndpointQueueNameFormatter != null
                ? _consumerEndpointQueueNameFormatter.Format(topicName, destinationQueue.EntityName)
                : $"Consumer.{destinationQueue.EntityName}.{EntityName}";


            var topic = builder.CreateTopic(EntityName, Durable, AutoDelete);

            var queue = builder.CreateQueue(consumerEndpointQueueName, destinationQueue.Durable, destinationQueue.AutoDelete);

            var consumer = builder.BindConsumer(topic, queue, Selector);
        }
    }
}
