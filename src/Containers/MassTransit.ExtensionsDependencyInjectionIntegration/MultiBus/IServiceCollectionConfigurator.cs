namespace MassTransit.ExtensionsDependencyInjectionIntegration.MultiBus
{
    using System;
    using Microsoft.Extensions.DependencyInjection;


    public interface IServiceCollectionConfigurator<in TBus> :
        IRegistrationConfigurator<IServiceProvider>
        where TBus : class, IBus
    {
        IServiceCollection Collection { get; }
    }
}
