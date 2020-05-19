namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    using System;
    using Microsoft.Extensions.DependencyInjection;


    public interface IServiceCollectionMediatorConfigurator :
        IMediatorRegistrationConfigurator<IServiceProvider>
    {
        IServiceCollection Collection { get; }
    }
}
