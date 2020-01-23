namespace MassTransit.SimpleInjectorIntegration.Registration
{
    using System;
    using MassTransit.Registration;
    using SimpleInjector;


    public class SimpleInjectorConfigurationServiceProvider :
        IConfigurationServiceProvider
    {
        readonly Container _provider;

        public SimpleInjectorConfigurationServiceProvider(Container provider)
        {
            _provider = provider;
        }

        public T GetRequiredService<T>()
            where T : class
        {
            return _provider.GetInstance<T>();
        }

        public T GetService<T>()
            where T : class
        {
            return _provider.TryGetInstance<T>();
        }

        public object GetService(Type serviceType)
        {
            IServiceProvider serviceProvider = _provider;

            return serviceProvider.GetService(serviceType);
        }
    }
}
