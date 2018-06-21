// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Lamar;
    using LamarIntegration;
    using NUnit.Framework;
    using Saga;
    using Scenarios;


    public class Lamar_Consumer :
        When_registering_a_consumer
    {
        [TearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        readonly IContainer _container;

        public Lamar_Consumer()
        {
            _container = new Container(x =>
            {
                x.For<SimpleConsumer>().Use<SimpleConsumer>();
                x.For<ISimpleConsumerDependency>().Use<SimpleConsumerDependency>();
                x.For<AnotherMessageConsumer>().Use<AnotherMessageConsumerImpl>();
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.LoadFrom(_container);
        }
    }


    public class Lamar_Saga :
        When_registering_a_saga
    {
        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        readonly IContainer _container;

        public Lamar_Saga()
        {
            _container = new Container(x =>
            {
                x.For(typeof(ISagaRepository<>))
                    .Use(typeof(InMemorySagaRepository<>))
                    .Singleton();
                //ConnectImplementationsToTypesClosing
                

                x.ForConcreteType<SimpleSaga>();
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.LoadFrom(_container);
        }

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _container.GetInstance<ISagaRepository<T>>();
        }
    }
}
