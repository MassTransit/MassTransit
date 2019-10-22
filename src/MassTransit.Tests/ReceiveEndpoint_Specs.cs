// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Creating_a_receive_endpoint_from_an_existing_bus :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_allowed()
        {
            Task<ConsumeContext<PingMessage>> pingHandled = null;

            var handle = Host.ConnectReceiveEndpoint("second_queue", x =>
            {
                pingHandled = Handled<PingMessage>(x);
            });
            await handle.Ready;
            try
            {
                await Bus.Publish(new PingMessage());

                var pinged = await pingHandled;

                Assert.That(pinged.ReceiveContext.InputAddress, Is.EqualTo(new Uri("loopback://localhost/second_queue")));
            }
            finally
            {
                await handle.StopAsync();
            }
        }
    }


    [TestFixture]
    public class Creating_a_receive_endpoint_from_an_existing_bus_twice :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_not_be_allowed_twice()
        {
            Task<ConsumeContext<PingMessage>> pingHandled = null;
            var handle = Host.ConnectReceiveEndpoint("second_queue", x =>
            {
                pingHandled = Handled<PingMessage>(x);
            });
            await handle.Ready;
            try
            {
                Assert.That(async () =>
                {
                    var unused = Host.ConnectReceiveEndpoint("second_queue", x =>
                    {
                    });
                    await unused.Ready;
                }, Throws.TypeOf<ConfigurationException>());
            }
            finally
            {
                await handle.StopAsync();
            }
        }
    }
}
