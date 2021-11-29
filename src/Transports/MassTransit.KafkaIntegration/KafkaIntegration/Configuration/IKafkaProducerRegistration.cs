namespace MassTransit.KafkaIntegration.Configuration
{
    public interface IKafkaProducerRegistration
    {
        void Register(IKafkaFactoryConfigurator configurator, IRiderRegistrationContext context);
    }
}
