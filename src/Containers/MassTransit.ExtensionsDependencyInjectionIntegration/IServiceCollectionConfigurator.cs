namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    using System;
    using Microsoft.Extensions.DependencyInjection;


    public interface IServiceCollectionConfigurator :
        IRegistrationConfigurator<IServiceProvider>
    {
        string Name { get; }
        IServiceCollection Collection { get; }
    }


    public interface IServiceCollectionConfigurator<in TBus> :
        IServiceCollectionConfigurator,
        IRegistrationConfigurator<TBus, IServiceProvider>
        where TBus : class
    {
    }
}
