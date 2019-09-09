namespace MassTransit.LamarIntegration.Registration
{
    using Lamar;
    using MassTransit.Registration;


    public class LamarConfigurationServiceProvider :
        IConfigurationServiceProvider
    {
        readonly IContainer _container;

        public LamarConfigurationServiceProvider(IContainer container)
        {
            _container = container;
        }

        public T GetRequiredService<T>()
            where T : class
        {
            return _container.GetInstance<T>();
        }

        public T GetService<T>()
            where T : class
        {
            return _container.TryGetInstance<T>();
        }
    }
}
