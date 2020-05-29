namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    using System;
    using MassTransit.Registration;
    using Microsoft.Extensions.DependencyInjection;


    public interface IServiceCollectionRiderConfigurator :
        IRiderConfigurator<IServiceProvider>
    {
        IServiceCollection Collection { get; }
    }
}
