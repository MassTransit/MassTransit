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
namespace MassTransit.Containers.Tests.DependencyInjection_Tests
{
    using System;
    using Common_Tests;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Saga;
    using Scenarios;


    [TestFixture]
    public class DependencyInjection_Saga :
        Common_Saga
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Saga()
        {
            var collection = new ServiceCollection();

            collection.AddMassTransit(x =>
            {
                x.AddSaga<SimpleSaga>();
                x.AddBus(provider => BusControl);
            });

            collection.AddSingleton<ISagaRepository<SimpleSaga>, InMemorySagaRepository<SimpleSaga>>();

            _provider = collection.BuildServiceProvider();
        }

        protected override void ConfigureSaga(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureSaga<SimpleSaga>(_provider);
        }

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _provider.GetService<ISagaRepository<T>>();
        }
    }


    [TestFixture]
    public class DependencyInjection_Saga_Endpoint :
        Common_Saga_Endpoint
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Saga_Endpoint()
        {
            var collection = new ServiceCollection();

            collection.AddMassTransit(x =>
            {
                x.AddSaga<SimpleSaga>()
                    .Endpoint(e => e.Name = "custom-endpoint-name");

                x.AddBus(provider => BusControl);
            });

            collection.AddSingleton<ISagaRepository<SimpleSaga>, InMemorySagaRepository<SimpleSaga>>();

            _provider = collection.BuildServiceProvider();
        }

        protected override void ConfigureEndpoints(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureEndpoints(_provider);
        }

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _provider.GetService<ISagaRepository<T>>();
        }
    }
}
