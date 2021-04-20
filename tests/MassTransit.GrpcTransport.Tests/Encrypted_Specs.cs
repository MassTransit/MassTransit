namespace MassTransit.GrpcTransport.Tests
{
    using System.Threading.Tasks;
    using MassTransit.Serialization;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Publishing_an_encrypted_message_to_an_endpoint_with_a_symmetric_key :
        GrpcTestFixture
    {
        [Test]
        public async Task Should_succeed()
        {
            await Bus.Publish(new PingMessage());

            ConsumeContext<PingMessage> received = await _handler;

            Assert.AreEqual(EncryptedMessageSerializerV2.EncryptedContentType, received.ReceiveContext.ContentType);
        }

        Task<ConsumeContext<PingMessage>> _handler;

        protected override void ConfigureGrpcReceiveEndpoint(IGrpcReceiveEndpointConfigurator configurator)
        {
            _handler = Handled<PingMessage>(configurator);
        }

        protected override void ConfigureGrpcBus(IGrpcBusFactoryConfigurator configurator)
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

            base.ConfigureGrpcBus(configurator);
        }
    }


    [TestFixture]
    public class Publishing_an_encrypted_message_to_an_endpoint_with_a_specific_key :
        GrpcTestFixture
    {
        [Test]
        public async Task Should_succeed()
        {
            await Bus.Publish(new PingMessage(), context => context.SetEncryptionKeyId("secure"));

            ConsumeContext<PingMessage> received = await _handler;

            Assert.AreEqual(EncryptedMessageSerializer.EncryptedContentType, received.ReceiveContext.ContentType);
        }

        Task<ConsumeContext<PingMessage>> _handler;

        protected override void ConfigureGrpcReceiveEndpoint(IGrpcReceiveEndpointConfigurator configurator)
        {
            _handler = Handled<PingMessage>(configurator);
        }

        protected override void ConfigureGrpcBus(IGrpcBusFactoryConfigurator configurator)
        {
            ISymmetricKeyProvider keyProvider = new TestSymmetricKeyProvider("secure");

            var streamProvider = new AesCryptoStreamProvider(keyProvider, "default");
            configurator.UseEncryptedSerializer(streamProvider);

            base.ConfigureGrpcBus(configurator);
        }
    }
}
