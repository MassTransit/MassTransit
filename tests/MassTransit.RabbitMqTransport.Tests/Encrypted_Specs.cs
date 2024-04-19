namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Serialization;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Publishing_an_encrypted_message_to_an_endpoint_with_a_symmetric_key :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_succeed()
        {
            await Bus.Publish(new PingMessage());

            ConsumeContext<PingMessage> received = await _handler;

            Assert.That(received.ReceiveContext.ContentType, Is.EqualTo(EncryptedMessageSerializerV2.EncryptedContentType));
        }

        Task<ConsumeContext<PingMessage>> _handler;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _handler = Handled<PingMessage>(configurator);
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
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

            configurator.UseEncryption(key);

            base.ConfigureRabbitMqBus(configurator);
        }
    }


    [TestFixture]
    public class Publishing_an_encrypted_message_to_an_endpoint_with_a_specific_key :
        RabbitMqTestFixture
    {
        [Test]
        public async Task Should_succeed()
        {
            await Bus.Publish(new PingMessage(), context => context.SetEncryptionKeyId("secure"));

            ConsumeContext<PingMessage> received = await _handler;

            Assert.That(received.ReceiveContext.ContentType, Is.EqualTo(EncryptedMessageSerializer.EncryptedContentType));
        }

        Task<ConsumeContext<PingMessage>> _handler;

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _handler = Handled<PingMessage>(configurator);
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            ISymmetricKeyProvider keyProvider = new TestSymmetricKeyProvider("secure");

            var streamProvider = new AesCryptoStreamProvider(keyProvider, "default");
            configurator.UseEncryptedSerializer(streamProvider);

            base.ConfigureRabbitMqBus(configurator);
        }
    }
}
