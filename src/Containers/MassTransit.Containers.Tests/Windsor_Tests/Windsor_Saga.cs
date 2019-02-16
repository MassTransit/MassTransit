// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Containers.Tests.Windsor_Tests
{
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Common_Tests;
    using NUnit.Framework;
    using Saga;
    using Scenarios;


    [TestFixture]
    public class Windsor_Saga :
        Common_Saga
    {
        readonly IWindsorContainer _container;

        public Windsor_Saga()
        {
            _container = new WindsorContainer();
            _container.AddMassTransit(x =>
            {
                x.AddSaga<SimpleSaga>();
                x.AddBus(provider => BusControl);
            });

            _container.Register(Component.For(typeof(ISagaRepository<>)).ImplementedBy(typeof(InMemorySagaRepository<>)).LifestyleSingleton());
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureSaga(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureSaga<SimpleSaga>(_container);
        }

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _container.Resolve<ISagaRepository<T>>();
        }
    }


    [TestFixture]
    public class Windsor_Saga_Endpoint :
        Common_Saga_Endpoint
    {
        readonly IWindsorContainer _container;

        public Windsor_Saga_Endpoint()
        {
            _container = new WindsorContainer();
            _container.AddMassTransit(x =>
            {
                x.AddSaga<SimpleSaga>()
                    .Endpoint(e => e.Name = "custom-endpoint-name");

                x.AddBus(provider => BusControl);
            });

            _container.Register(Component.For(typeof(ISagaRepository<>)).ImplementedBy(typeof(InMemorySagaRepository<>)).LifestyleSingleton());
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureEndpoints(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureEndpoints(_container);
        }

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _container.Resolve<ISagaRepository<T>>();
        }
    }
}
