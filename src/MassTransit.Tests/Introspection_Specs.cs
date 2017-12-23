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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Introspection;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Probing_the_bus :
        InMemoryTestFixture
    {
        Task<ConsumeContext<PingMessage>> _handled;

        [Test, Explicit]
        public async Task Should_return_a_wonderful_breakdown_of_the_guts_inside_it()
        {
            await Bus.Publish(new PingMessage());

            await _handled;

            ProbeResult result = Bus.GetProbeResult();

            Console.WriteLine(result.ToJsonString());
        }

        [Test]
        public void Should_extract_receive_endpoint_addresses()
        {
            List<Uri> receiveAddresses = Bus.GetReceiveEndpointAddresses().ToList();
            Assert.Contains(InputQueueAddress, receiveAddresses);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Handler<PingMessage>(async context =>
            {
            }, x =>
            {
                x.UseRateLimit(100, TimeSpan.FromSeconds(1));
                x.UseConcurrencyLimit(32);
            });

            _handled = Handled<PingMessage>(configurator);

            var consumer = new MultiTestConsumer(TestTimeout);

            consumer.Consume<PingMessage>();
            consumer.Consume<PongMessage>();

            consumer.Configure(configurator);
        }
    }
}