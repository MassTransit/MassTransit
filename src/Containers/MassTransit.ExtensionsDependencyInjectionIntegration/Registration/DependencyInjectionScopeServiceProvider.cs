namespace MassTransit.ExtensionsDependencyInjectionIntegration.Registration
{
    using System;
    using MassTransit.Registration;
    using Microsoft.Extensions.DependencyInjection;


    public class DependencyInjectionScopeServiceProvider :
        IScopeServiceProvider
    {
        readonly IServiceProvider _provider;

        public DependencyInjectionScopeServiceProvider(IServiceProvider provider)
        {
            _provider = provider;
        }

        public T GetRequiredService<T>()
            where T : class
        {
            return _provider.GetRequiredService<T>();
        }

        public T GetService<T>()
            where T : class
        {
            return _provider.GetService<T>();
        }

        public object GetService(Type serviceType)
        {
            return _provider.GetService(serviceType);
        }
    }
}
