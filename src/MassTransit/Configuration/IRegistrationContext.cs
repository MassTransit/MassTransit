namespace MassTransit
{
    public interface IRegistrationContext<out TContainerContext> :
        IRegistration
        where TContainerContext : class
    {
        TContainerContext Container { get; }

        void UseHealthCheck(IBusFactoryConfigurator configurator);
    }


    public interface IRegistrationContext<in TBus, out TContainerContext> :
        IRegistrationContext<TContainerContext>
        where TBus : IBus
        where TContainerContext : class
    {
    }
}
