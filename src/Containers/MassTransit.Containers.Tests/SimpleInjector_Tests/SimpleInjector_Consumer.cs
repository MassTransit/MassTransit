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
namespace MassTransit.Containers.Tests.SimpleInjector_Tests
{
    using System;
    using System.Threading.Tasks;
    using Common_Tests;
    using GreenPipes;
    using NUnit.Framework;
    using Scenarios;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;


    [TestFixture]
    public class SimpleInjector_Consumer :
        Common_Consumer
    {
        readonly Container _container;

        public SimpleInjector_Consumer()
        {
            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            _container.AddMassTransit(cfg =>
                {
                    cfg.AddConsumer<SimpleConsumer>();
                    cfg.AddBus(() => BusControl);
                });

            _container.Register<ISimpleConsumerDependency, SimpleConsumerDependency>(Lifestyle.Scoped);

            _container.Register<AnotherMessageConsumer, AnotherMessageConsumerImpl>(Lifestyle.Scoped);
        }

        [TearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureConsumer(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<SimpleConsumer>(_container);
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseLog(Console.Out, log =>
                Task.FromResult(
                    $"Received (input_queue): {log.Context.ReceiveContext.TransportHeaders.Get("MessageId", "N/A")}, Types = ({string.Join(",", log.Context.SupportedMessageTypes)})"));

            base.ConfigureInMemoryBus(configurator);
        }
    }


    [TestFixture]
    public class SimpleInjector_Consumer_Endpoint :
        Common_Consumer_Endpoint
    {
        readonly Container _container;

        public SimpleInjector_Consumer_Endpoint()
        {
            _container = new Container();
            _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            _container.AddMassTransit(cfg =>
            {
                cfg.AddConsumer<SimplerConsumer>()
                    .Endpoint(e => e.Name = "custom-endpoint-name");

                cfg.AddBus(() => BusControl);
            });
        }

        [TearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureEndpoints(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureEndpoints(_container);
        }
    }
}
