namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    using System;
    using Microsoft.Extensions.DependencyInjection;


    public interface IServiceCollectionMediatorConfigurator :
        IMediatorConfigurator<IServiceProvider>
    {
        IServiceCollection Collection { get; }
    }
}
