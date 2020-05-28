namespace MassTransit.Registration
{
    using Riders;


    public interface IRiderRegistrationContext<out TContainerContext> :
        IRegistration
        where TContainerContext : class
    {
        TContainerContext Container { get; }

        void UseHealthCheck(IRiderFactoryConfigurator configurator);
    }
}
