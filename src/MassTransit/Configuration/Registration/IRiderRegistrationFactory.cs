namespace MassTransit.Registration
{
    public interface IRiderRegistrationFactory<in TContainerContext>
        where TContainerContext : class
    {
        IBusInstanceSpecification CreateRider(IRiderRegistrationContext<TContainerContext> context);
    }
}
