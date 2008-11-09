namespace MassTransit.UnityIntegration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Practices.Unity;
    using ServiceBus;

    public class UnityObjectBuilder :
        IObjectBuilder
    {
        private readonly UnityContainer _container;

        public UnityObjectBuilder(UnityContainer container)
        {
            _container = container;
        }

        public object GetService(Type serviceType)
        {
            return _container.Resolve(serviceType);
        }

        public object GetInstance(Type serviceType)
        {
            return _container.Resolve(serviceType);
        }

        public object GetInstance(Type serviceType, string key)
        {
            return _container.Resolve(serviceType, key);
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.ResolveAll(serviceType);
        }

        public TService GetInstance<TService>()
        {
            return _container.Resolve<TService>();
        }

        public TService GetInstance<TService>(string key)
        {
            return _container.Resolve<TService>(key);
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return _container.ResolveAll<TService>();
        }

        public T GetInstance<T>(IDictionary arguments)
        {
            throw new System.NotImplementedException();
        }

        public void Release<T>(T obj)
        {
            _container.Teardown(obj);
        }
    }
}