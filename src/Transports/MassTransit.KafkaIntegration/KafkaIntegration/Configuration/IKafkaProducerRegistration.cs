namespace MassTransit.KafkaIntegration.Configuration
{
    using MassTransit.Configuration;


    public interface IKafkaProducerRegistration :
        IRegistration
    {
        void Register(IKafkaFactoryConfigurator configurator, IRiderRegistrationContext context);
    }
}
