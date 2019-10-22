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
namespace MassTransit.Containers.Tests.Autofac_Tests
{
    using Autofac;
    using Common_Tests;
    using NUnit.Framework;
    using Scenarios;


    [TestFixture]
    public class Autofac_Consumer :
        Common_Consumer
    {
        readonly IContainer _container;

        public Autofac_Consumer()
        {
            var builder = new ContainerBuilder();
            builder.AddMassTransit(x =>
            {
                x.AddConsumer<SimpleConsumer>();

                x.AddBus(provider => BusControl);
            });

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

        protected override void ConfigureConsumer(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<SimpleConsumer>(_container);
        }
    }


    [TestFixture]
    public class Autofac_Consumer_Endpoint :
        Common_Consumer_Endpoint
    {
        readonly IContainer _container;

        public Autofac_Consumer_Endpoint()
        {
            var builder = new ContainerBuilder();
            builder.AddMassTransit(x =>
            {
                x.AddConsumer<SimplerConsumer>()
                    .Endpoint(e => e.Name = "custom-endpoint-name");

                x.AddBus(provider => BusControl);
            });

            _container = builder.Build();
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
    }


    [TestFixture]
    public class Autofac_Consumer_ServiceEndpoint :
        Common_Consumer_ServiceEndpoint
    {
        readonly IContainer _container;

        public Autofac_Consumer_ServiceEndpoint()
        {
            var builder = new ContainerBuilder();
            builder.AddMassTransit(x =>
            {
                x.AddConsumer<PingRequestConsumer>();

                x.AddBus(provider => BusControl);
            });

            _container = builder.Build();
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureEndpoints(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ServiceInstance(x => x.ConfigureEndpoints(_container));
        }
    }
}
