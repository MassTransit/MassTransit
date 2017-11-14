// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Containers.Tests
{
    using NUnit.Framework;
    using Saga;
    using Scenarios;
    using Unity;
    using Unity.Lifetime;


    public class Unity_Consumer :
        When_registering_a_consumer
    {
        [TearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        readonly IUnityContainer _container;

        public Unity_Consumer()
        {
            _container = new UnityContainer();
            _container.RegisterType<SimpleConsumer>(new HierarchicalLifetimeManager());
            _container.RegisterType<ISimpleConsumerDependency, SimpleConsumerDependency>(
                new HierarchicalLifetimeManager());
            _container.RegisterType<AnotherMessageConsumer, AnotherMessageConsumerImpl>(
                new HierarchicalLifetimeManager());
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.LoadFrom(_container);
        }
    }


    public class Unity_Saga :
        When_registering_a_saga
    {
        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        readonly IUnityContainer _container;

        public Unity_Saga()
        {
            _container = new UnityContainer();
            _container.RegisterType<SimpleSaga>();
            _container.RegisterType(typeof(ISagaRepository<>), typeof(InMemorySagaRepository<>),
                new ContainerControlledLifetimeManager());
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.LoadFrom(_container);
        }

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _container.Resolve<ISagaRepository<T>>();
        }
    }
}