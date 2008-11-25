namespace MassTransit.StructureMapIntegration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    
    using StructureMap;

    public class StructureMapObjectBuilder :
        IObjectBuilder
    {
        public T GetInstance<T>(IDictionary arguments)
        {
            return ObjectFactory.GetInstance<T>(); //how to handle component
        }

        public void Release<T>(T obj)
        {
            //structure map doesn't have this concept.
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