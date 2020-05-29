namespace MassTransit.Registration
{
    public interface IRegistrationRiderFactory<in TContainerContext>
        where TContainerContext : class
    {
        IBusInstanceSpecification CreateRider(IRiderRegistrationContext<TContainerContext> context);
    }
}
