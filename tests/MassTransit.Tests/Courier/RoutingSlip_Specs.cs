// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.Courier
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Courier;
    using MassTransit.Courier.Contracts;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Sending_a_routing_slip :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_properly_serialized_as_a_message()
        {
            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddActivity("test", new Uri("loopback://localhost/execute_testactivity"), new {});

            await InputQueueSendEndpoint.Send(builder.Build());

            await _received;
        }

        Task<ConsumeContext<RoutingSlip>> _received;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _received = Handled<RoutingSlip>(configurator);
        }
    }
}