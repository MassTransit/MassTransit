namespace MassTransit
{
    using Confluent.Kafka;


    /// <summary>
    /// Set configuration and ProducerConfig
    /// </summary>
    /// <typeparam name="TProducer">Type is inherit from IKafkaProducer</typeparam>
    public abstract class KafkaProducerConfiguration<TProducer> where TProducer : class, IKafkaProducer
    {
        /// <summary>
        /// Set configuration for kafka producer
        /// </summary>
        /// <param name="context"></param>
        /// <param name="configurator"></param>
        public abstract void Configure(IRiderRegistrationContext context,
            IKafkaProducerConfigurator<Null, TProducer> configurator);

        public virtual string TopicName => typeof(TProducer).Name;

        /// <summary>
        /// Producer config
        /// </summary>
        public ProducerConfig ProducerConfig { get; set; }
    }
}
