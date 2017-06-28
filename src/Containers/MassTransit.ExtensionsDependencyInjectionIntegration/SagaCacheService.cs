// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ExtensionsDependencyInjectionIntegration
{
    using MassTransit.Saga;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SagaCacheService : ISagaCacheService
    {
        private static ConcurrentDictionary<Type, ICachedConfigurator> _consumerHandlers = new ConcurrentDictionary<Type, ICachedConfigurator>();

        public void Add<T>()
            where T : class, ISaga
        {
            _consumerHandlers.GetOrAdd(typeof(T), _ => new CachedEndPointSagaConfigurator<T>());
        }

        public IEnumerable<ICachedConfigurator> GetHandlers()
        {
            return _consumerHandlers.Values.ToList();
        }

        public ConcurrentDictionary<Type, ICachedConfigurator> Instance
        {
            get
            {
                return _consumerHandlers;
            }
        }

        public void Configure(Type consumerType, IReceiveEndpointConfigurator configurator, IServiceProvider container)
        {
            _consumerHandlers
                .GetOrAdd(consumerType, _ => (ICachedConfigurator)Activator.CreateInstance(typeof(ICachedConfigurator).MakeGenericType(consumerType)))
                .Configure(configurator, container);
        }
    }

    public class CachedEndPointSagaConfigurator<T> : ICachedConfigurator
        where T : class, ISaga
    {
        public void Configure(IReceiveEndpointConfigurator configurator, IServiceProvider services)
        {
            var sagaRepository = services.GetService(typeof(ISagaRepository<T>)) as ISagaRepository<T>;

            var structureMapSagaRepository = new ExtensionsDependencyInjectionSagaRepository<T>(sagaRepository, services);

            configurator.Saga(structureMapSagaRepository);
        }
    }

    public interface ISagaCacheService
    {
        void Add<T>() where T : class, ISaga;
        IEnumerable<ICachedConfigurator> GetHandlers();
        ConcurrentDictionary<Type, ICachedConfigurator> Instance { get; }
        void Configure(Type consumerType, IReceiveEndpointConfigurator configurator, IServiceProvider container);
    }
}
