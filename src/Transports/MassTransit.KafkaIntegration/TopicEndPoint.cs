namespace MassTransit
{
    using Confluent.Kafka;


    public abstract class TopicEndPoint
        <TProducer> where TProducer : class, IKafkaProducer
    {
        protected TopicEndPoint(IRiderRegistrationContext context)
        {
        }
        public virtual string GroupId { get; }


        public virtual string TopicName => typeof(TProducer).Name;
        public ConsumerConfig ConsumerConfig { get; set; }

        protected abstract void ActionMethod(IKafkaTopicReceiveEndpointConfigurator<Ignore, TProducer> configurator);


    }
}

