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
    using Ninject;
    using Ninject.Extensions.NamedScope;
    using NUnit.Framework;
    using Saga;
    using Scenarios;


    public class Ninject_Consumer :
        When_registering_a_consumer
    {
        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        readonly IKernel _container;

        public Ninject_Consumer()
        {
            _container = new StandardKernel();
            
            _container.ConfigureMassTransit(x => x.AddConsumer<SimpleConsumer>());
            
            _container.Bind<ISimpleConsumerDependency>()
                .To<SimpleConsumerDependency>().InNamedScope("message");
            _container.Bind<AnotherMessageConsumer>()
                .To<AnotherMessageConsumerImpl>().InNamedScope("message");
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.LoadFrom(_container);
        }
    }

    /// <summary>
    /// This works, but fails in the test fixture for some reason.
    /// </summary>
    [TestFixture]
    public class Ninject_Saga :
        When_registering_a_saga
    {
        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        readonly IKernel _container;

        public Ninject_Saga()
        {
            _container = new StandardKernel();
            
            _container.ConfigureMassTransit(x => x.AddSaga<SimpleSaga>());
            
            _container.Bind<ISagaRepository<SimpleSaga>>()
                .To<InMemorySagaRepository<SimpleSaga>>()
                .InSingletonScope();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.LoadFrom(_container);
        }

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _container.Get<ISagaRepository<T>>();
        }
    }
}