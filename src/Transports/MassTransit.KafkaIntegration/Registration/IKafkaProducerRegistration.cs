namespace MassTransit.KafkaIntegration.Registration
{
    public interface IKafkaProducerRegistration
    {
        void Register(IKafkaFactoryConfigurator configurator);
    }
}
