namespace MassTransit.Registration
{
    using Riders;


    public interface IRegistrationRiderFactory<in TContainerContext, in TRider>
        where TContainerContext : class
        where TRider : IRider
    {
        IBusInstanceSpecification CreateRider(IRiderRegistrationContext<TContainerContext> context);
    }
}
