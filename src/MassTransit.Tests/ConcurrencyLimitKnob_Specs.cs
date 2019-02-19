// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using GreenPipes;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Using_a_concurrency_limit_on_a_receive_endpoint :
        InMemoryTestFixture
    {
        IManagementEndpointConfigurator _management;

        [Test]
        public async Task Should_allow_reconfiguration()
        {
            var client = Bus.CreateRequestClient<SetConcurrencyLimit>();

            var response = await client.GetResponse<ConcurrencyLimitUpdated>(new
            {
                ConcurrencyLimit = 16,
                Timestamp = DateTime.UtcNow
            });
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            _management = configurator.ManagementEndpoint();

            base.ConfigureInMemoryBus(configurator);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            configurator.UseConcurrencyLimit(32, _management);
        }
    }
}
