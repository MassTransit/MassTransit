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
    using Autofac;
    using NUnit.Framework;
    using Saga;
    using Scenarios;


    public class Autofac_Consumer :
        When_registering_a_consumer
    {
        readonly IContainer _container;

        public Autofac_Consumer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<SimpleConsumer>();
            builder.RegisterType<SimpleConsumerDependency>()
                .As<ISimpleConsumerDependency>();
            builder.RegisterType<AnotherMessageConsumerImpl>()
                .As<AnotherMessageConsumer>();

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureInputQueueEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.LoadFrom(_container);
        }
    }

    public class Autofac_Consumer_by_interface :
        When_registering_a_consumer_by_interface
    {
        readonly IContainer _container;

        public Autofac_Consumer_by_interface()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<SimpleConsumer>()
                .As<IConsumer<SimpleMessageInterface>>();
            builder.RegisterType<SimpleConsumerDependency>()
                .As<ISimpleConsumerDependency>();
            builder.RegisterType<AnotherMessageConsumerImpl>()
                .As<AnotherMessageConsumer>();

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureInputQueueEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.LoadFrom(_container);
        }
    }


    public class Autofac_Saga :
        When_registering_a_saga
    {
        public Autofac_Saga()
        {
            var builder = new ContainerBuilder();
            builder.RegisterGeneric(typeof(InMemorySagaRepository<>))
                .As(typeof(ISagaRepository<>))
                .SingleInstance();
            builder.RegisterType<SimpleSaga>();

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        readonly IContainer _container;

        protected override void ConfigureInputQueueEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.LoadFrom(_container);
        }

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _container.Resolve<ISagaRepository<T>>();
        }
    }
}