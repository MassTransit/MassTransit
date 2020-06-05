namespace MassTransit.KafkaIntegration.Specifications
{
    using GreenPipes;
    using MassTransit.Registration;
    using Transport;


    public interface IKafkaProducerSpecification :
        ISpecification
    {
        IKafkaProducerFactory CreateProducerFactory(IBusInstance busInstance);
    }
}
