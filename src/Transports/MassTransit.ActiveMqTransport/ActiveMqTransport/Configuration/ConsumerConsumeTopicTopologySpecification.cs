namespace MassTransit.ActiveMqTransport.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using Topology;


    public class ConsumerConsumeTopicTopologySpecification :
        ActiveMqTopicBindingConfigurator,
        IActiveMqConsumeTopologySpecification
    {
        public ConsumerConsumeTopicTopologySpecification(string topicName, bool durable = true, bool autoDelete = false)
            : base(topicName, durable, autoDelete)
        {
        }

        /// <summary>
        /// If set to <c>true</c>, the consumer is shared between multiple receive endpoints.
        /// It means if you have multiple instances of the application a broker will balance messages between them.
        /// </summary>
        /// <remarks>
        /// Shared Durable subscription is supported only by ActiveMQ Artemis Broker and AMQP. If your broker is ActiveMQ
        /// or you are using OpenWire protocol (tcp|ssl://host:port or activemq://host:port URI) this is not supported.
        /// </remarks>
        public bool Shared { get; set; }

        /// <summary>
        /// The consumer name, if specified
        /// </summary>
        public string ConsumerName { get; set; } = null;

        public IEnumerable<ValidationResult> Validate()
        {
            return Enumerable.Empty<ValidationResult>();
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            var topic = builder.CreateTopic(EntityName, Durable, AutoDelete);

            _ = builder.BindConsumer(topic, null, Selector, ConsumerName, Shared);
        }
    }
}
