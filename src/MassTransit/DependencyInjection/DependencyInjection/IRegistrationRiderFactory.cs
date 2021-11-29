namespace MassTransit.DependencyInjection
{
    using Configuration;
    using Transports;


    public interface IRegistrationRiderFactory<in TRider>
        where TRider : IRider
    {
        IBusInstanceSpecification CreateRider(IRiderRegistrationContext context);
    }
}
