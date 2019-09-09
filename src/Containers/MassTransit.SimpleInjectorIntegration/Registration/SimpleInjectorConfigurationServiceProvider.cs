namespace MassTransit.SimpleInjectorIntegration.Registration
{
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
    }
}
