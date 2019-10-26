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
namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Introspection;
    using Hosting;
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
    public class Publishing_a_message_to_an_endpoint_with_a_slash :
        AzureServiceBusTestFixture
    {
        public Publishing_a_message_to_an_endpoint_with_a_slash()
            : base(serviceUri: AzureServiceBusEndpointUriCreator.Create(Configuration.ServiceNamespace))
        {
        }

        Task<ConsumeContext<PingMessage>> _handler;

        protected override void ConfigureServiceBusBusHost(IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host)
        {
            configurator.ReceiveEndpoint(host, "test_endpoint_scope/input_queue", e =>
            {
                _handler = Handled<PingMessage>(e);
            });
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
            var key = new byte[]
            {
                31, 182, 254, 29, 98, 114, 85, 168, 176, 48, 113,
                206, 198, 176, 181, 125, 106, 134, 98, 217, 113,
                158, 88, 75, 118, 223, 117, 160, 224, 1, 47, 162
            };

            var keyProvider = new ConstantSecureKeyProvider(key);

            var streamProvider = new AesCryptoStreamProviderV2(keyProvider);
            configurator.UseEncryptedSerializerV2(streamProvider);

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

            Assert.AreEqual(EncryptedMessageSerializerV2.EncryptedContentType, received.ReceiveContext.ContentType);
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
            configurator.ReceiveEndpoint(e =>
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
