namespace MassTransit.ExtensionsDependencyInjectionIntegration.MultiBus
{
    using MassTransit.Registration;
    using Microsoft.Extensions.DependencyInjection;


    public interface IServiceCollectionRiderConfigurator<in TBus> :
        IRiderRegistrationConfigurator
        where TBus : class, IBus
    {
        IServiceCollection Collection { get; }
    }
}
