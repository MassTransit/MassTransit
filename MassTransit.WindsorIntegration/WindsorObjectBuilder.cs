namespace MassTransit.WindsorIntegration
{
    using System;
    using System.Collections;
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

        public object Build(Type objectType)
        {
            return _container.Resolve(objectType);
        }

        public T Build<T>() where T : class
        {
            return _container.Resolve<T>();
        }


        public T Build<T>(Type component) where T : class
        {
            return _container[component] as T;
        }

        public T Build<T>(IDictionary arguments)
        {
            return _container.Resolve<T>(arguments);
        }

        public void Release<T>(T obj)
        {
        	_container.ReleaseComponent(obj);
        }
    }
}
