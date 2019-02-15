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
    using Scenarios;


    [TestFixture]
    public class DependencyInjection_Consumer :
        Common_Consumer
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Consumer()
        {
            var collection = new ServiceCollection();
            collection.AddMassTransit(x =>
            {
                x.AddConsumer<SimpleConsumer>();
                x.AddBus(provider => BusControl);
            });

            collection.AddScoped<ISimpleConsumerDependency, SimpleConsumerDependency>();
            collection.AddScoped<AnotherMessageConsumer, AnotherMessageConsumerImpl>();

            _provider = collection.BuildServiceProvider();
        }

        protected override void ConfigureConsumer(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConfigureConsumer<SimpleConsumer>(_provider);
        }
    }


    [TestFixture]
    public class DependencyInjection_Consumer_Endpoint :
        Common_Consumer_Endpoint
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Consumer_Endpoint()
        {
            var collection = new ServiceCollection();
            collection.AddMassTransit(x =>
            {
                x.AddConsumer<SimplerConsumer>()
                    .Endpoint(e => e.Name = "custom-endpoint-name");

                x.AddBus(provider => BusControl);
            });

            collection.AddScoped<ISimpleConsumerDependency, SimpleConsumerDependency>();
            collection.AddScoped<AnotherMessageConsumer, AnotherMessageConsumerImpl>();

            _provider = collection.BuildServiceProvider();
        }

        protected override void ConfigureEndpoints(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureEndpoints(_provider);
        }
    }
}
