namespace MassTransit.KafkaIntegration.Configuration
{
    using Transports;


    public interface IKafkaProducerSpecification :
        ISpecification
    {
        string TopicName { get; }
        IKafkaProducerFactory CreateProducerFactory(IBusInstance busInstance);
    }
}
