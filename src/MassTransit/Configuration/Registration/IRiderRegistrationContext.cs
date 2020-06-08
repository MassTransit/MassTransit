namespace MassTransit.Registration
{
    using Riders;


    public interface IRiderRegistrationContext :
        IRegistration
    {
        void UseHealthCheck(IRiderFactoryConfigurator configurator);
    }
}
