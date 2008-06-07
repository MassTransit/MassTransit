namespace MassTransit.WindsorIntegration
{
    using System;
    using Castle.Windsor;
    using ServiceBus;

    public class WindsorObjectBuilder :
        IObjectBuilder
    {
        private WindsorContainer _container;


        public WindsorObjectBuilder(WindsorContainer container)
        {
            _container = container;
        }

        public object Build(Type objectType)
        {
            return _container.Resolve(objectType);
        }

        public T Build<T>(Type type) where T : class
        {
            return _container.Resolve<T>();
        }

        public void Release<T>(T obj)
        {
            _container.Release(obj);
        }
    }
}
