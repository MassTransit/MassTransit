namespace MassTransit.ExtensionsDependencyInjectionIntegration.MultiBus
{
    using Microsoft.Extensions.DependencyInjection;


    public interface IServiceCollectionConfigurator<in TBus> :
        IBusRegistrationConfigurator
        where TBus : class, IBus
    {
        IServiceCollection Collection { get; }
    }
}
