namespace MassTransit
{
    public interface IRegistrationContext<in TBus, out TContainerContext> :
        IRegistration
        where TBus : IBus
        where TContainerContext : class
    {
        TContainerContext Container { get; }

        void UseHealthCheck(IBusFactoryConfigurator configurator);
    }
}
