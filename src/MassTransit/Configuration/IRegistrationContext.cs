namespace MassTransit
{
    public interface IRegistrationContext<out TContainerContext> :
        IRegistration
        where TContainerContext : class
    {
        TContainerContext Container { get; }

        void UseHealthCheck(IBusFactoryConfigurator configurator);
    }
}
