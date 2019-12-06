namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    using System;
    using Microsoft.Extensions.DependencyInjection;


    public interface IServiceCollectionConfigurator :
        IRegistrationConfigurator<IServiceProvider>
    {
        IServiceCollection Collection { get; }
    }
}
