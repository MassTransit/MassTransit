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
namespace MassTransit.AzureServiceBusTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Introspection;
    using NUnit.Framework;
    using Serialization;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Publishing_a_message_to_an_endpoint :
        AzureServiceBusTestFixture
    {
        Task<ConsumeContext<PingMessage>> _handler;

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _handler = Handled<PingMessage>(configurator);
        }

        [Test, Explicit]
        public void Should_return_a_wonderful_breakdown_of_the_guts_inside_it()
        {
            ProbeResult result = Bus.GetProbeResult();

            Console.WriteLine(result.ToJsonString());
        }

        [Test]
        public async Task Should_succeed()
        {
            await Bus.Publish(new PingMessage());

            await _handler;
        }
    }


    [TestFixture]
    public class Publishing_a_message_to_an_endpoint_from_another_scope :
        TwoScopeAzureServiceBusTestFixture
    {
        Task<ConsumeContext<PingMessage>> _handler;
        Task<ConsumeContext<PingMessage>> _secondHandler;

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _handler = Handled<PingMessage>(configurator);
        }

        protected override void ConfigureSecondInputQueueEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _secondHandler = Handled<PingMessage>(configurator);
        }

        [Test]
        public async Task Should_succeed()
        {
            await SecondBus.Publish(new PingMessage());

            await _handler;

            await _secondHandler;
        }
    }


    [TestFixture]
    public class Publishing_an_encrypted_message_to_an_endpoint :
        AzureServiceBusTestFixture
    {
        Task<ConsumeContext<PingMessage>> _handler;

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _handler = Handled<PingMessage>(configurator);
        }

        protected override void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
            ISymmetricKeyProvider keyProvider = new TestSymmetricKeyProvider();
            var streamProvider = new AesCryptoStreamProvider(keyProvider, "default");
            configurator.UseEncryptedSerializer(streamProvider);

            base.ConfigureServiceBusBus(configurator);
        }

        [Test, Explicit]
        public void Should_return_a_wonderful_breakdown_of_the_guts_inside_it()
        {
            ProbeResult result = Bus.GetProbeResult();

            Console.WriteLine(result.ToJsonString());
        }

        [Test]
        public async Task Should_succeed()
        {
            await Bus.Publish(new PingMessage());

            ConsumeContext<PingMessage> received = await _handler;

            Assert.AreEqual(EncryptedMessageSerializer.EncryptedContentType, received.ReceiveContext.ContentType);
        }
    }


    [TestFixture]
    public class Publishing_a_message_to_an_remove_subscriptions_endpoint :
        AzureServiceBusTestFixture
    {
        Task<ConsumeContext<PingMessage>> _consumer;

        protected override void ConfigureServiceBusBusHost(IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host)
        {
            base.ConfigureServiceBusBusHost(configurator, host);
            configurator.ReceiveEndpoint(host, e =>
            {
                e.RemoveSubscriptions = true;
                _consumer = HandledByConsumer<PingMessage>(e);
            });
        }

        [Test]
        public async Task Should_succeed()
        {
            await Bus.Publish(new PingMessage());

            await _consumer;
        }
    }
}
