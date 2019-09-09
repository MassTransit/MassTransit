namespace MassTransit.ExtensionsDependencyInjectionIntegration.Registration
{
    using System;
    using MassTransit.Registration;
    using Microsoft.Extensions.DependencyInjection;


    public class DependencyInjectionConfigurationServiceProvider :
        IConfigurationServiceProvider
    {
        readonly IServiceProvider _provider;

        public DependencyInjectionConfigurationServiceProvider(IServiceProvider provider)
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
    }
}
