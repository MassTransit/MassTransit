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
namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using AzureServiceBusTransport.Tests;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class Creating_a_receive_endpoint_from_an_existing_bus :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_be_allowed()
        {
            Task<ConsumeContext<PingMessage>> pingHandled = null;

            var handle = await Host.ConnectReceiveEndpoint("second_queue", x =>
            {
                pingHandled = Handled<PingMessage>(x);

                x.RemoveSubscriptions = true;
            });
            try
            {
                await Bus.Publish(new PingMessage());

                ConsumeContext<PingMessage> pinged = await pingHandled;

                Assert.That(pinged.ReceiveContext.InputAddress, Is.EqualTo(new Uri(Host.Address, "second_queue")));
            }
            finally
            {
                await handle.StopAsync();
            }
        }

        [Test]
        public async Task Should_not_be_allowed_twice()
        {
            var handle = await Host.ConnectReceiveEndpoint("second_queue", x =>
            {
                Handled<PingMessage>(x);
            });
            try
            {
                Assert.That(async () =>
                {
                    var unused = await Host.ConnectReceiveEndpoint("second_queue", x =>
                    {
                    });
                }, Throws.TypeOf<ConfigurationException>());
            }
            finally
            {
                await handle.StopAsync();
            }
        }
    }


    [TestFixture]
    public class Creating_a_subscription_endpoint_from_an_existing_bus :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_be_allowed()
        {
            Task<ConsumeContext<PingMessage>> pingHandled = null;

            var handle = await Host.ConnectSubscriptionEndpoint<PingMessage>("second_subscription", x =>
            {
                pingHandled = Handled<PingMessage>(x);
            });
            try
            {
                await Bus.Publish(new PingMessage());

                ConsumeContext<PingMessage> pinged = await pingHandled;

                Assert.That(pinged.ReceiveContext.InputAddress,
                    Is.EqualTo(new Uri(Host.Address, string.Join("/", Host.MessageNameFormatter.GetMessageName(typeof(PingMessage)), "second_subscription"))));
            }
            finally
            {
                await handle.StopAsync();
            }
        }

        [Test]
        public async Task Should_not_be_allowed_twice()
        {
            var handle = await Host.ConnectSubscriptionEndpoint<PingMessage>("second_subscription", x =>
            {
                Handled<PingMessage>(x);
            });
            try
            {
                Assert.That(async () =>
                {
                    var unused = await Host.ConnectSubscriptionEndpoint<PingMessage>("second_subscription", x =>
                    {
                    });
                }, Throws.TypeOf<ConfigurationException>());
            }
            finally
            {
                await handle.StopAsync();
            }
        }
    }
}