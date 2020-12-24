namespace MassTransit.KafkaIntegration.Specifications
{
    using GreenPipes;
    using MassTransit.Registration;
    using Transport;


    public interface IKafkaProducerSpecification :
        ISpecification
    {
        string TopicName { get; }
        IKafkaProducerFactory CreateProducerFactory(IBusInstance busInstance);
    }
}
