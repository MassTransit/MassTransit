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
    using System;
    using System.Threading.Tasks;
    using Autofac;
    using NUnit.Framework;
    using Saga;
    using Scenarios;
    using Shouldly;
    using TestFramework;


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

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.LoadFrom(_container);
        }
    }


    [TestFixture]
    public class Autofac_Consumer_with_endpoint :
        InMemoryTestFixture
    {
        readonly IContainer _container;

        public Autofac_Consumer_with_endpoint()
        {
            var builder = new ContainerBuilder();
            builder.AddMassTransit(x =>
            {
                x.AddConsumer<SimpleConsumer>()
                    .Endpoint(e =>
                    {
                        e.Name = "frankly-simple";
                        e.Temporary = true;
                        e.ConcurrentMessageLimit = 20;
                    });

                x.AddBus(provider => BusControl);
            });

            builder.RegisterType<SimpleConsumerDependency>()
                .As<ISimpleConsumerDependency>();

            builder.RegisterType<AnotherMessageConsumerImpl>()
                .As<AnotherMessageConsumer>();

            _container = builder.Build();
        }

        [Test]
        public async Task Should_receive_using_the_first_consumer()
        {
            const string name = "Joe";

            var sendEndpoint = await Bus.GetSendEndpoint(new Uri("loopback://localhost/frankly-simple"));

            await sendEndpoint.Send(new SimpleMessageClass(name));

            SimpleConsumer lastConsumer = await SimpleConsumer.LastConsumer;
            lastConsumer.ShouldNotBe(null);
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureEndpoints(_container);
        }
    }


    public class AutofacRegistration_Saga :
        When_registering_a_saga
    {
        public AutofacRegistration_Saga()
        {
            var builder = new ContainerBuilder();
            builder.AddMassTransit(cfg =>
            {
                cfg.AddSaga<SimpleSaga>();
            });

            builder.RegisterInMemorySagaRepository();

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        readonly IContainer _container;

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
