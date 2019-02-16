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
namespace MassTransit.Containers.Tests.Lamar_Tests
{
    using Common_Tests;
    using Lamar;
    using NUnit.Framework;
    using Saga;
    using Scenarios;


    [TestFixture]
    public class Lamar_Saga :
        Common_Saga
    {
        readonly IContainer _container;

        public Lamar_Saga()
        {
            _container = new Container(registry =>
            {
                registry.AddMassTransit(cfg =>
                {
                    cfg.AddSaga<SimpleSaga>();
                    cfg.AddBus(context => BusControl);
                });

                registry.For(typeof(ISagaRepository<>))
                    .Use(typeof(InMemorySagaRepository<>))
                    .Singleton();
            });
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
            return _container.GetInstance<ISagaRepository<T>>();
        }
    }


    [TestFixture]
    public class Lamar_Saga_Endpoint :
        Common_Saga_Endpoint
    {
        readonly IContainer _container;

        public Lamar_Saga_Endpoint()
        {
            _container = new Container(registry =>
            {
                registry.AddMassTransit(cfg =>
                {
                    cfg.AddSaga<SimpleSaga>()
                        .Endpoint(e => e.Name = "custom-endpoint-name");

                    cfg.AddBus(context => BusControl);
                });

                registry.For(typeof(ISagaRepository<>))
                    .Use(typeof(InMemorySagaRepository<>))
                    .Singleton();
            });
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
            return _container.GetInstance<ISagaRepository<T>>();
        }
    }
}
