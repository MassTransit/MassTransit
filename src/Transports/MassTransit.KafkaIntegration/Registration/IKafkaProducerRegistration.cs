namespace MassTransit.KafkaIntegration.Registration
{
    using MassTransit.Registration;


    public interface IKafkaProducerRegistration
    {
        void Register(IKafkaFactoryConfigurator configurator, IRiderRegistrationContext context);
    }
}
