namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    using Microsoft.Extensions.DependencyInjection;


    public interface IServiceCollectionBusConfigurator :
        IBusRegistrationConfigurator
    {
        IServiceCollection Collection { get; }
    }
}
