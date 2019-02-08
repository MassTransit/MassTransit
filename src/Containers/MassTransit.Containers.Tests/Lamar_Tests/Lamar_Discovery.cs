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
    using System;
    using Common_Tests;
    using Common_Tests.Discovery;
    using Lamar;
    using NUnit.Framework;
    using TestFramework.Messages;


    public class Lamar_Discovery :
        Common_Discovery
    {
        readonly IContainer _container;

        public Lamar_Discovery()
        {
            _container = new Container(registry =>
            {
                registry.AddMassTransit(x =>
                {
                    x.AddConsumersFromNamespaceContaining(typeof(DiscoveryTypes));
                    x.AddSagaStateMachinesFromNamespaceContaining(typeof(DiscoveryTypes));
                    x.AddSagasFromNamespaceContaining(typeof(DiscoveryTypes));
                    x.AddActivitiesFromNamespaceContaining(typeof(DiscoveryTypes));

                    x.AddBus(provider => BusControl);
                    x.AddRequestClient<PingMessage>(new Uri("loopback://localhost/ping-queue"));
                });

                registry.RegisterInMemorySagaRepository<DiscoveryPingSaga>();
                registry.RegisterInMemorySagaRepository<DiscoveryPingState>();
            });
        }

        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override IRequestClient<PingMessage> GetRequestClient()
        {
            return _container.GetInstance<IRequestClient<PingMessage>>();
        }

        protected override void ConfigureEndpoints(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.ConfigureEndpoints(_container);
        }
    }
}
