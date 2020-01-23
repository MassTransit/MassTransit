namespace MassTransit.StructureMapIntegration.Registration
{
    using System;
    using MassTransit.Registration;
    using StructureMap;


    public class StructureMapConfigurationServiceProvider :
        IConfigurationServiceProvider
    {
        readonly IContainer _container;

        public StructureMapConfigurationServiceProvider(IContainer container)
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

        public object GetService(Type serviceType)
        {
            return _container.TryGetInstance(serviceType);
        }
    }
}
