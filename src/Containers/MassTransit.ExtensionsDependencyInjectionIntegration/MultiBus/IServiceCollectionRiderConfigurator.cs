namespace MassTransit.ExtensionsDependencyInjectionIntegration.MultiBus
{
    using System;
    using MassTransit.Registration;
    using Microsoft.Extensions.DependencyInjection;


    public interface IServiceCollectionRiderConfigurator<in TBus> :
        IRiderConfigurator<IServiceProvider>
        where TBus : class, IBus
    {
        IServiceCollection Collection { get; }
    }
}
