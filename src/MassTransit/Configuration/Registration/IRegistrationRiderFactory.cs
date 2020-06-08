namespace MassTransit.Registration
{
    using Riders;


    public interface IRegistrationRiderFactory<in TRider>
        where TRider : IRider
    {
        IBusInstanceSpecification CreateRider(IRiderRegistrationContext context);
    }
}
