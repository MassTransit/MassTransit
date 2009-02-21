namespace MassTransit.NinjectIntegration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Ninject;
    using Ninject.Parameters;

    public class NinjectObjectBuilder :
        IObjectBuilder
    {
        private readonly IKernel _kernel;

        public NinjectObjectBuilder(IKernel kernel)
        {
            _kernel = kernel;
        }

        public object GetService(Type serviceType)
        {
            return _kernel.Get(serviceType);
        }

        public object GetInstance(Type serviceType)
        {
            return _kernel.Get(serviceType);
        }

        public object GetInstance(Type serviceType, string key)
        {
            return _kernel.Get(serviceType, key);
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }

        public TService GetInstance<TService>()
        {
            return _kernel.Get<TService>();
        }

        public TService GetInstance<TService>(string key)
        {
            return _kernel.Get<TService>(key);
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return _kernel.GetAll<TService>();
        }

        public T GetInstance<T>(IDictionary arguments)
        {
            var args = new List<ConstructorArgument>();
            foreach (DictionaryEntry entry in arguments)
            {
                args.Add(new ConstructorArgument((string)entry.Key, entry.Value));
            }
            return _kernel.Get<T>(args.ToArray());
        }

        public void Release<T>(T obj)
        {
            //I think we only do this on transient objects?
        }
    }
}