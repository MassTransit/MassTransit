namespace MassTransit.ExtensionsDependencyInjectionIntegration.MultiBus
{
    using System;
    using Microsoft.Extensions.DependencyInjection;


    public interface IServiceCollectionConfigurator<in TBus> :
        IRegistrationConfigurator<TBus, IServiceProvider>
        where TBus : class, IBus
    {
        IServiceCollection Collection { get; }
    }
}
