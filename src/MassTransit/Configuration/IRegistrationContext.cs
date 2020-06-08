namespace MassTransit
{
    public interface IRegistrationContext :
        IRegistration
    {
        void UseHealthCheck(IBusFactoryConfigurator configurator);
    }
}
