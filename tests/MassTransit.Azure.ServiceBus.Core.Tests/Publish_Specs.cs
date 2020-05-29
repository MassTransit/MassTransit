namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using NUnit.Framework;
    using Serialization;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Publishing_a_message_to_an_endpoint :
        AzureServiceBusTestFixture
    {
        [Test]
        [Explicit]
        public void Should_return_a_wonderful_breakdown_of_the_guts_inside_it()
        {
            var result = Bus.GetProbeResult();

            Console.WriteLine(result.ToJsonString());
        }

        [Test]
        public async Task Should_succeed()
        {
            await Bus.Publish(new PingMessage());

            await _handler;
        }

        Task<ConsumeContext<PingMessage>> _handler;

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _handler = Handled<PingMessage>(configurator);
        }
    }


    [TestFixture]
    public class Publishing_a_message_to_an_endpoint_with_a_slash :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_succeed()
        {
            await Bus.Publish(new PingMessage());

            await _handler;
        }

        public Publishing_a_message_to_an_endpoint_with_a_slash()
            : base(serviceUri: AzureServiceBusEndpointUriCreator.Create(Configuration.ServiceNamespace))
        {
        }

        Task<ConsumeContext<PingMessage>> _handler;

        protected override void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint("test_endpoint_scope/input_queue", e =>
            {
                _handler = Handled<PingMessage>(e);
            });
        }
    }


    [TestFixture]
    public class Publishing_a_message_to_an_endpoint_from_another_scope :
        TwoScopeAzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_succeed()
        {
            await SecondBus.Publish(new PingMessage());

            await _handler;

            await _secondHandler;
        }

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
    }


    [TestFixture]
    public class Publishing_an_encrypted_message_to_an_endpoint :
        AzureServiceBusTestFixture
    {
        [Test]
        [Explicit]
        public void Should_return_a_wonderful_breakdown_of_the_guts_inside_it()
        {
            var result = Bus.GetProbeResult();

            Console.WriteLine(result.ToJsonString());
        }

        [Test]
        public async Task Should_succeed()
        {
            await Bus.Publish(new PingMessage());

            ConsumeContext<PingMessage> received = await _handler;

            Assert.AreEqual(EncryptedMessageSerializerV2.EncryptedContentType, received.ReceiveContext.ContentType);
        }

        Task<ConsumeContext<PingMessage>> _handler;

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _handler = Handled<PingMessage>(configurator);
        }

        protected override void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
            var key = new byte[]
            {
                31,
                182,
                254,
                29,
                98,
                114,
                85,
                168,
                176,
                48,
                113,
                206,
                198,
                176,
                181,
                125,
                106,
                134,
                98,
                217,
                113,
                158,
                88,
                75,
                118,
                223,
                117,
                160,
                224,
                1,
                47,
                162
            };

            var keyProvider = new ConstantSecureKeyProvider(key);

            var streamProvider = new AesCryptoStreamProviderV2(keyProvider);
            configurator.UseEncryptedSerializerV2(streamProvider);

            base.ConfigureServiceBusBus(configurator);
        }
    }


    [TestFixture]
    public class Publishing_a_message_to_an_remove_subscriptions_endpoint :
        AzureServiceBusTestFixture
    {
        [Test]
        public async Task Should_succeed()
        {
            await Bus.Publish(new PingMessage());

            await _consumer;
        }

        Task<ConsumeContext<PingMessage>> _consumer;

        protected override void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
            configurator.ReceiveEndpoint(e =>
            {
                e.RemoveSubscriptions = true;
                _consumer = HandledByConsumer<PingMessage>(e);
            });
        }
    }
}
