namespace MassTransit.WindsorIntegration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Castle.Core;
    using Castle.MicroKernel;
    using ServiceBus;

    public class WindsorObjectBuilder :
        IObjectBuilder
    {
        private readonly IKernel _container;


        public WindsorObjectBuilder(IKernel container)
        {
            _container = container;
        }

        public T GetInstance<T>(IDictionary arguments)
        {
            return _container.Resolve<T>(arguments);
        }

        public void Release<T>(T obj)
        {
        	_container.ReleaseComponent(obj);
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
            return _container.Resolve(key, serviceType);
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            var x = _container.GetHandlers(serviceType);
            ArrayList result = new ArrayList();
            foreach (var o in x)
            {
                result.Add(o.Resolve(CreationContext.Empty));
            }
            return result.ToArray();
        }

        public TService GetInstance<TService>()
        {
            return _container.Resolve<TService>();
        }

        public TService GetInstance<TService>(string key)
        {
            return (TService)_container.Resolve(key, typeof(TService));
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            var x = _container.GetHandlers(typeof(TService));
            var result = new List<TService>();
            foreach (var o in x)
            {
                result.Add((TService)o.Resolve(CreationContext.Empty));
            }
            return result.ToArray();
        }
    }
}
