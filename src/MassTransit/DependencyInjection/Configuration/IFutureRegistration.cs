namespace MassTransit.Configuration
{
    public interface IFutureRegistration :
        IRegistration
    {
        void Configure(IReceiveEndpointConfigurator configurator, IRegistrationContext context);

        IFutureDefinition GetDefinition(IRegistrationContext context);
    }
}
