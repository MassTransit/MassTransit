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
    using Scenarios;


    [TestFixture]
    public class Windsor_Consumer :
        Common_Consumer
    {
        readonly IWindsorContainer _container;

        public Windsor_Consumer()
        {
            var container = new WindsorContainer();
            container.AddMassTransit(x =>
            {
                x.AddConsumer<SimpleConsumer>();
                x.AddBus(provider => BusControl);
            });

            container.Register(Component.For<ISimpleConsumerDependency>().ImplementedBy<SimpleConsumerDependency>().LifestyleScoped(),
                Component.For<AnotherMessageConsumer>().ImplementedBy<AnotherMessageConsumerImpl>().LifestyleScoped()
            );

            _container = container;
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override MassTransit.IRegistration Registration => _container.Resolve<MassTransit.IRegistration>();
    }


    [TestFixture]
    public class Windsor_Consumer_Endpoint :
        Common_Consumer_Endpoint
    {
        readonly IWindsorContainer _container;

        public Windsor_Consumer_Endpoint()
        {
            var container = new WindsorContainer();
            container.AddMassTransit(x =>
            {
                x.AddConsumer<SimplerConsumer>()
                    .Endpoint(e => e.Name = "custom-endpoint-name");

                x.AddBus(provider => BusControl);
            });

            container.Register(Component.For<ISimpleConsumerDependency>().ImplementedBy<SimpleConsumerDependency>().LifestyleScoped(),
                Component.For<AnotherMessageConsumer>().ImplementedBy<AnotherMessageConsumerImpl>().LifestyleScoped()
            );

            _container = container;
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override MassTransit.IRegistration Registration => _container.Resolve<MassTransit.IRegistration>();
    }
}
