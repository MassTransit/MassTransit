// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using Magnum.Extensions;

    public class ServiceContainer :
        IServiceContainer
    {
        readonly IServiceBus _bus;
        readonly Dictionary<Type, IBusService> _services;
        bool _disposed;

        public ServiceContainer(IServiceBus bus)
        {
            _bus = bus;
            _services = new Dictionary<Type, IBusService>();
        }

        public TService GetService<TService>()
        {
            var type = typeof(TService);

            if (_services.ContainsKey(type))
                return (TService) _services[type];

            throw new InvalidOperationException("The service '{0}' is not registered on the bus".FormatWith(type.Name));
        }

        public void AddService(Type serviceType, IBusService service)
        {
            _services.Add(serviceType, service);
        }

        public void Start()
        {
            foreach (var service in _services.Values)
            {
                service.Start(_bus);
            }
        }

        public void Stop()
        {
            foreach (var service in _services.Values)
            {
                service.Stop();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                foreach (var service in _services.Values)
                {
                    service.Dispose();
                }
                _services.Clear();
            }
            _disposed = true;
        }

        ~ServiceContainer()
        {
            Dispose(false);
        }
    }
}