namespace MassTransit.StructureMapIntegration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using ServiceBus;
    using StructureMap;

    public class StructureMapObjectBuilder :
        IObjectBuilder
    {
        public object Build(Type objectType)
        {
            return ObjectFactory.GetInstance(objectType);
        }

        public T Build<T>() where T : class
        {
            return ObjectFactory.GetInstance<T>();
        }

        public T Build<T>(Type component) where T : class
        {
            return ObjectFactory.GetInstance<T>(); //how to handle component
        }

        public T Build<T>(IDictionary arguments)
        {
            return ObjectFactory.GetInstance<T>(); //how to handle component
        }

        public void Release<T>(T obj)
        {
            //what to do
        }

        public void Register<T>() where T : class
        {
            //TODO: Is this correct?
            StructureMapConfiguration.ForRequestedType<T>().TheDefaultIsConcreteType<T>();
        }

        public object GetService(Type serviceType)
        {
            return ObjectFactory.GetInstance(serviceType);
        }

        public object GetInstance(Type serviceType)
        {
            return ObjectFactory.GetInstance(serviceType);
        }

        public object GetInstance(Type serviceType, string key)
        {
            return ObjectFactory.GetNamedInstance(serviceType, key);
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            var result = new ArrayList(ObjectFactory.GetAllInstances(serviceType));
            return result.ToArray();
        }

        public TService GetInstance<TService>()
        {
            return ObjectFactory.GetInstance<TService>();
        }

        public TService GetInstance<TService>(string key)
        {
            return ObjectFactory.GetNamedInstance<TService>(key);
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return ObjectFactory.GetAllInstances<TService>();
        }
    }
}