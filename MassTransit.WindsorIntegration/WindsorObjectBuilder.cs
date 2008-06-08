namespace MassTransit.WindsorIntegration
{
    using System;
    using System.Collections;
    using Castle.Windsor;
    using ServiceBus;

    public class WindsorObjectBuilder :
        IObjectBuilder
    {
        private readonly WindsorContainer _container;


        public WindsorObjectBuilder(WindsorContainer container)
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
            _container.Release(obj);
        }
    }
}
