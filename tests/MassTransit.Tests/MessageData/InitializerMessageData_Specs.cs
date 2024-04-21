namespace MassTransit.Tests.MessageData
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using MassTransit.MessageData;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Initializing_a_message_with_a_message_data_property :
        InMemoryTestFixture
    {
        [Test]
        public async Task A_missing_body_should_fault()
        {
            IRequestClient<ProcessDocument> client = CreateRequestClient<ProcessDocument>();

            const string byteData = "This byte is a good byte to eat.";
            const string stringValue = "This string is soo incredibly huge.";
            const string byteValue = "Such a byte value, a really great byte value.";

            Assert.That(async () => await client.GetResponse<DocumentProcessed>(new
            {
                InVar.CorrelationId,
                ByteData = byteData,
                StringValue = stringValue,
                ByteValue = Encoding.UTF8.GetBytes(byteValue),
            }), Throws.TypeOf<RequestFaultException>());
        }

        [Test]
        public async Task Should_load_the_data_from_the_repository()
        {
            IRequestClient<ProcessDocument> client = CreateRequestClient<ProcessDocument>();

            const string stringData = "This is a huge string, and it is just too big to fit.";
            const string byteData = "This byte is a good byte to eat.";
            const string stringValue = "This string is soo incredibly huge.";
            const string byteValue = "Such a byte value, a really great byte value.";

            byte[] streamBytes = new byte[1000];
            using MemoryStream ms = new MemoryStream(streamBytes);

            Response<DocumentProcessed> response = await client.GetResponse<DocumentProcessed>(new
            {
                InVar.CorrelationId,
                StringData = stringData,
                ByteData = byteData,
                StringValue = stringValue,
                ByteValue = Encoding.UTF8.GetBytes(byteValue),
                StreamData = ms
            });

            Assert.That(response.Message.StringData, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(response.Message.StringData.HasValue, Is.True);
                Assert.That(response.Message.StringData.Address, Is.EqualTo(_stringDataAddress), "Should use the existing message data address");
            });
            var text = await response.Message.StringData.Value;
            Assert.Multiple(() =>
            {
                Assert.That(text, Is.EqualTo(stringData));

                Assert.That(response.Message.StringByteData, Is.Not.Null);
            });
            Assert.Multiple(() =>
            {
                Assert.That(response.Message.StringByteData.HasValue, Is.True);
                Assert.That(response.Message.StringByteData.Address, Is.EqualTo(_stringDataAddress), "Should use the existing message data address");
            });
            byte[] bytes = await response.Message.StringByteData.Value;
            Assert.Multiple(() =>
            {
                Assert.That(Encoding.UTF8.GetString(bytes), Is.EqualTo(stringData));

                Assert.That(response.Message.ByteData, Is.Not.Null);
            });
            Assert.Multiple(() =>
            {
                Assert.That(response.Message.ByteData.HasValue, Is.True);
                Assert.That(response.Message.ByteData.Address, Is.EqualTo(_byteDataAddress), "Should use the existing message data address");
            });
            bytes = await response.Message.ByteData.Value;
            Assert.Multiple(() =>
            {
                Assert.That(Encoding.UTF8.GetString(bytes), Is.EqualTo(byteData));

                Assert.That(response.Message.StreamData, Is.Not.Null);
            });
            Assert.Multiple(() =>
            {
                Assert.That(response.Message.StreamData.HasValue, Is.True);
                Assert.That(response.Message.StreamData.Address, Is.EqualTo(_streamDataAddress), "Should use the existing message data address");
            });
            using MemoryStream receivedStream = new MemoryStream();
            var stream = await response.Message.StreamData.Value;
            await stream.CopyToAsync(receivedStream);
            Assert.Multiple(() =>
            {
                Assert.That(receivedStream.ToArray(), Is.EqualTo(streamBytes));

                Assert.That(response.Message.StringValue, Is.Not.Null);
            });
            Assert.That(response.Message.StringValue.HasValue, Is.True);
            text = await response.Message.StringValue.Value;
            Assert.Multiple(() =>
            {
                Assert.That(text, Is.EqualTo(stringValue));

                Assert.That(response.Message.StringByteValue, Is.Not.Null);
            });
            Assert.That(response.Message.StringByteValue.HasValue, Is.True);
            bytes = await response.Message.StringByteValue.Value;
            Assert.Multiple(() =>
            {
                Assert.That(Encoding.UTF8.GetString(bytes), Is.EqualTo(stringValue));

                Assert.That(response.Message.ByteValue, Is.Not.Null);
            });
            Assert.That(response.Message.ByteValue.HasValue, Is.True);
            bytes = await response.Message.ByteValue.Value;
            Assert.That(Encoding.UTF8.GetString(bytes), Is.EqualTo(byteValue));
        }

        readonly IMessageDataRepository _repository = new InMemoryMessageDataRepository();
        Uri _stringDataAddress;
        Uri _byteDataAddress;
        Uri _streamDataAddress;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseMessageData(_repository);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Handler<ProcessDocument>(async context =>
            {
                if (!context.Message.StringData.HasValue)
                    throw new ArgumentException("StringData was not present.");

                var stringData = await context.Message.StringData.Value;
                if (string.IsNullOrWhiteSpace(stringData))
                    throw new ArgumentException("StringData was empty.");

                _stringDataAddress = context.Message.StringData.Address;
                _byteDataAddress = context.Message.ByteData.Address;
                _streamDataAddress = context.Message.StreamData.Address;

                await context.RespondAsync<DocumentProcessed>(new
                {
                    context.Message.CorrelationId,
                    context.Message.StringData,
                    StringByteData = context.Message.StringData,
                    context.Message.ByteData,
                    context.Message.StringValue,
                    StringByteValue = context.Message.StringValue,
                    context.Message.ByteValue,
                    context.Message.StreamData
                });
            });
        }


        public interface ProcessDocument
        {
            Guid CorrelationId { get; }
            MessageData<string> StringData { get; }
            MessageData<byte[]> ByteData { get; }
            string StringValue { get; }
            byte[] ByteValue { get; }
            MessageData<Stream> StreamData { get; }
        }


        public interface DocumentProcessed
        {
            Guid CorrelationId { get; }
            MessageData<string> StringData { get; }
            MessageData<byte[]> StringByteData { get; }
            MessageData<byte[]> ByteData { get; }
            MessageData<string> StringValue { get; }
            MessageData<byte[]> StringByteValue { get; }
            MessageData<byte[]> ByteValue { get; }
            MessageData<Stream> StreamData { get; }
        }
    }
}
