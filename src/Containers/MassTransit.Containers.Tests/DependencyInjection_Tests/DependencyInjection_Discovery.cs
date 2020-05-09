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
    using Common_Tests.Discovery;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class DependencyInjection_Discovery :
        Common_Discovery
    {
        readonly IServiceProvider _provider;

        public DependencyInjection_Discovery()
        {
            var collection = new ServiceCollection();
            collection.AddMassTransit(x =>
            {
                x.AddConsumersFromNamespaceContaining(typeof(DiscoveryTypes));
                x.AddSagaStateMachinesFromNamespaceContaining(typeof(DiscoveryTypes));
                x.AddSagasFromNamespaceContaining(typeof(DiscoveryTypes));
                x.AddActivitiesFromNamespaceContaining(typeof(DiscoveryTypes));

                x.AddBus(provider => BusControl);
                x.AddRequestClient<PingMessage>(new Uri("loopback://localhost/ping-queue"));
            });

            collection.RegisterInMemorySagaRepository<DiscoveryPingSaga>();
            collection.RegisterInMemorySagaRepository<DiscoveryPingState>();

            _provider = collection.BuildServiceProvider(true);
        }

        protected override IRequestClient<PingMessage> GetRequestClient()
        {
            return _provider.CreateRequestClient<PingMessage>();
        }
        protected override IRegistration Registration => _provider.GetRequiredService<IRegistration>();
    }
}
