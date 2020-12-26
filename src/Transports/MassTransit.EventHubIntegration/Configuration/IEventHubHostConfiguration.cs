namespace MassTransit.EventHubIntegration
{
    using Contexts;
    using GreenPipes;
    using MassTransit.Registration;


    public interface IEventHubHostConfiguration :
        ISpecification
    {
        IConnectionContextSupervisor ConnectionContextSupervisor { get; }

        IEventHubRider Build(IRiderRegistrationContext context, IBusInstance busInstance);
    }
}
