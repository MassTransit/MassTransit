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
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Scenarios;


    [TestFixture]
    public class Lamar_Consumer :
        Common_Consumer
    {
        readonly IContainer _container;

        public Lamar_Consumer()
        {
            _container = new Container(registry =>
            {
                registry.AddMassTransit(cfg =>
                {
                    cfg.AddConsumer<SimpleConsumer>();
                    cfg.AddBus(context => BusControl);
                });

                registry.For<ISimpleConsumerDependency>()
                    .Use<SimpleConsumerDependency>();

                registry.For<AnotherMessageConsumer>()
                    .Use<AnotherMessageConsumerImpl>();
            });
        }

        [TearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IRegistration Registration => _container.GetRequiredService<IRegistration>();
    }


    [TestFixture]
    public class Lamar_Consumer_Endpoint :
        Common_Consumer_Endpoint
    {
        readonly IContainer _container;

        public Lamar_Consumer_Endpoint()
        {
            _container = new Container(registry =>
            {
                registry.AddMassTransit(cfg =>
                {
                    cfg.AddConsumer<SimplerConsumer>()
                        .Endpoint(e => e.Name = "custom-endpoint-name");

                    cfg.AddBus(context => BusControl);
                });
            });
        }

        [TearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IRegistration Registration => _container.GetRequiredService<IRegistration>();
    }
}
