namespace MassTransit.Registration
{
    public interface IRegistrationBusFactory<in TContainerContext>
        where TContainerContext : class
    {
        IBusInstance CreateBus(IRegistrationContext<TContainerContext> context);
    }
}
