// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.WindsorIntegration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Castle.MicroKernel;
    

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
            var x = _container.ResolveAll(serviceType, new {});
            ArrayList result = new ArrayList();
            foreach (var o in x)
            {
                result.Add(o);
            }
            return result.ToArray();
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
    }
}