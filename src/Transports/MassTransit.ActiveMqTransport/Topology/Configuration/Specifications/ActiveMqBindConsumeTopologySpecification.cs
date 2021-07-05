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
    public class ActiveMqBindConsumeTopologySpecification :
        TopicBindingConfigurator,
        IActiveMqBindingConsumeTopologySpecification
    {
        //readonly string _consumerName;

        public ActiveMqBindConsumeTopologySpecification(string topicName, bool durable = true, bool autoDelete = false)
            : base(topicName, durable, autoDelete)
        {
            //_consumerName = consumerName;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return Enumerable.Empty<ValidationResult>();
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            var consumerName = $"Consumer.{{queue}}.{EntityName}";

            var topic = builder.CreateTopic(EntityName, Durable, AutoDelete);

            var destinationQueue = builder.Queue.Queue;

            var consumerQueueName = consumerName.Replace("{queue}", destinationQueue.EntityName);

            var queue = builder.CreateQueue(consumerQueueName, destinationQueue.Durable, destinationQueue.AutoDelete);

            var consumer = builder.BindConsumer(topic, queue, Selector);
        }
    }

    public class ArtemisBindConsumeTopologySpecification :
        TopicBindingConfigurator,
        IActiveMqBindingConsumeTopologySpecification
    {
        public ArtemisBindConsumeTopologySpecification(string topicName, bool durable = true, bool autoDelete = false)
            : base(topicName, durable, autoDelete)
        {
        }

        public ArtemisBindConsumeTopologySpecification(Topic topic)
            : base(topic)
        {
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return Enumerable.Empty<ValidationResult>();
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            //var consumerName = $"{EntityName}::Consumer.{{queue}}.{EntityName}";
            var consumerName = $"{EntityName}::Consumer.{{queue}}.{EntityName}";

            var topic = builder.CreateTopic(EntityName, Durable, AutoDelete);

            var destinationQueue = builder.Queue.Queue;

            var consumerQueueName = consumerName.Replace("{queue}", destinationQueue.EntityName);

            var queue = builder.CreateQueue(consumerQueueName, destinationQueue.Durable, destinationQueue.AutoDelete);

            var consumer = builder.BindConsumer(topic, queue, Selector);
        }
    }
}
