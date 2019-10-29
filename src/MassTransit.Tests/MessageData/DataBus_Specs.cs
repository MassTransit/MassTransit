namespace MassTransit.Tests.MessageData
{
    namespace DataBus_Specs
    {
        using System;
        using System.Collections.Generic;
        using System.IO;
        using System.Text;
        using System.Threading.Tasks;
        using MassTransit.MessageData;
        using MassTransit.Serialization;
        using NUnit.Framework;
        using Shouldly;
        using TestFramework;


        [TestFixture]
        public class Sending_a_large_message_through_the_file_system :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_load_the_data_from_the_repository()
            {
                string data = NewId.NextGuid().ToString();

                var message = new SendMessageWithBigData {Body = await _repository.PutString(data)};

                await InputQueueSendEndpoint.Send(message);

                await _received;

                _receivedBody.ShouldBe(data);
            }

            [Test]
            public async Task Should_be_able_to_write_bytes_too()
            {
                byte[] data = NewId.NextGuid().ToByteArray();

                var message = new MessageWithByteArrayImpl {Bytes = await _repository.PutBytes(data)};

                await InputQueueSendEndpoint.Send(message);

                await _receivedBytes;

                _receivedBytesArray.ShouldBe(data);
            }

            IMessageDataRepository _repository;
            Task<ConsumeContext<MessageWithBigData>> _received;
            Task<ConsumeContext<MessageWithByteArray>> _receivedBytes;
            string _receivedBody;
            byte[] _receivedBytesArray;

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                string messageDataPath = Path.Combine(baseDirectory, "MessageData");

                var dataDirectory = new DirectoryInfo(messageDataPath);

                _repository = new FileSystemMessageDataRepository(dataDirectory);

                configurator.UseMessageData(_repository);

                _received = Handler<MessageWithBigData>(configurator, async context =>
                {
                    _receivedBody = await context.Message.Body.Value;
                });

                configurator.UseMessageData(_repository);

                _receivedBytes = Handler<MessageWithByteArray>(configurator, async context =>
                {
                    _receivedBytesArray = await context.Message.Bytes.Value;
                });
            }
        }


        [TestFixture]
        public class Sending_a_large_message_through_the_file_system_encrypted :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_load_the_data_from_the_repository()
            {
                string data = NewId.NextGuid().ToString();

                var message = new SendMessageWithBigData {Body = await _repository.PutString(data)};

                await InputQueueSendEndpoint.Send(message);

                await _received;

                _receivedBody.ShouldBe(data);
            }

            [Test]
            public async Task Should_be_able_to_write_bytes_too()
            {
                byte[] data = NewId.NextGuid().ToByteArray();

                var message = new MessageWithByteArrayImpl {Bytes = await _repository.PutBytes(data)};

                await InputQueueSendEndpoint.Send(message);

                await _receivedBytes;

                _receivedBytesArray.ShouldBe(data);
            }

            IMessageDataRepository _repository;
            Task<ConsumeContext<MessageWithBigData>> _received;
            Task<ConsumeContext<MessageWithByteArray>> _receivedBytes;
            string _receivedBody;
            byte[] _receivedBytesArray;

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                string messageDataPath = Path.Combine(baseDirectory, "MessageData");

                var dataDirectory = new DirectoryInfo(messageDataPath);

                var fileRepository = new FileSystemMessageDataRepository(dataDirectory);

                ISymmetricKeyProvider keyProvider = new TestSymmetricKeyProvider();
                var cryptoStreamProvider = new AesCryptoStreamProvider(keyProvider, "default");
                _repository = new EncryptedMessageDataRepository(fileRepository, cryptoStreamProvider);

                configurator.UseMessageData(_repository);

                _received = Handler<MessageWithBigData>(configurator, async context =>
                {
                    _receivedBody = await context.Message.Body.Value;
                });

                configurator.UseMessageData(_repository);

                _receivedBytes = Handler<MessageWithByteArray>(configurator, async context =>
                {
                    _receivedBytesArray = await context.Message.Bytes.Value;
                });
            }
        }


        [TestFixture]
        public class Receiving_a_large_message_with_data :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_load_the_data_from_the_repository()
            {
                string data = NewId.NextGuid().ToString();
                Uri dataAddress;
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data), false))
                {
                    dataAddress = await _messageDataRepository.Put(stream);
                }

                var message = new SendMessageWithBigData {Body = new ConstantMessageData<string>(dataAddress, data)};

                await InputQueueSendEndpoint.Send(message);

                await _received;

                _receivedBody.ShouldBe(data);
            }

            IMessageDataRepository _messageDataRepository;

            Task<ConsumeContext<MessageWithBigData>> _received;
            string _receivedBody;

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                _messageDataRepository = new InMemoryMessageDataRepository();

                configurator.UseMessageData(_messageDataRepository);

                _received = Handler<MessageWithBigData>(configurator, async context =>
                {
                    _receivedBody = await context.Message.Body.Value;
                });
            }
        }


        [TestFixture]
        public class Receiving_nested_message_with_data :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_load_the_data_from_the_repository()
            {
                string data = NewId.NextGuid().ToString();
                Uri dataAddress;
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                using (var stream = new MemoryStream(buffer, false))
                {
                    dataAddress = await _messageDataRepository.Put(stream);
                }

                var message = new Message
                {
                    Document = new Document() {Body = new ConstantMessageData<byte[]>(dataAddress, buffer)},
                    Documents = new IDocument[]
                    {
                        new Document()
                        {
                            Title = "Hello, World",
                            PageCount = 27,
                            Body = new ConstantMessageData<byte[]>(dataAddress, buffer)
                        }
                    },
                    DocumentList = new List<IDocument>()
                    {
                        new Document()
                        {
                            Title = "Hello, World",
                            PageCount = 44,
                            Body = new ConstantMessageData<byte[]>(dataAddress, buffer)
                        }
                    },
                    DocumentIndex = new Dictionary<string, IDocument>()
                    {
                        {"First", new Document {Body = new ConstantMessageData<byte[]>(dataAddress, buffer)}},
                        {"Second", new Document {Body = new ConstantMessageData<byte[]>(dataAddress, buffer)}},
                    }
                };

                await InputQueueSendEndpoint.Send(message);

                await _received;

                Assert.That(_body, Is.Not.Null);
                Assert.That(_body0, Is.Not.Null);

                var result = await _body.Value;
                Assert.That(result, Is.EqualTo(buffer));

                result = await _body0.Value;
                Assert.That(result, Is.EqualTo(buffer));

                result = await _bodyList0.Value;
                Assert.That(result, Is.EqualTo(buffer));

                result = await _bodyFirst.Value;
                Assert.That(result, Is.EqualTo(buffer));

                result = await _bodySecond.Value;
                Assert.That(result, Is.EqualTo(buffer));
            }

            IMessageDataRepository _messageDataRepository;

            Task<ConsumeContext<IMessage>> _received;
            MessageData<byte[]> _body;
            MessageData<byte[]> _body0;
            MessageData<byte[]> _bodyList0;
            MessageData<byte[]> _bodyFirst;
            MessageData<byte[]> _bodySecond;

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                _messageDataRepository = new InMemoryMessageDataRepository();

                configurator.UseMessageData(_messageDataRepository);

                _received = Handler<IMessage>(configurator, async context =>
                {
                    _body = context.Message.Document.Body;
                    _body0 = context.Message.Documents[0].Body;
                    _bodyList0 = context.Message.DocumentList[0].Body;
                    _bodyFirst = context.Message.DocumentIndex["First"].Body;
                    _bodySecond = context.Message.DocumentIndex["Second"].Body;
                });
            }
        }


        [TestFixture]
        public class Receiving_a_large_message_with_data_bytes :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_load_the_data_from_the_repository()
            {
                Guid nextGuid = NewId.NextGuid();
                string data = nextGuid.ToString();
                Uri dataAddress;
                using (var stream = new MemoryStream(nextGuid.ToByteArray(), false))
                {
                    dataAddress = await _messageDataRepository.Put(stream);
                }

                var message = new MessageWithByteArrayImpl {Bytes = new ConstantMessageData<byte[]>(dataAddress, nextGuid.ToByteArray())};

                await InputQueueSendEndpoint.Send(message);

                await _received;

                var newId = new NewId(_receivedBytesArray);
                newId.ToString().ShouldBe(data);
            }

            IMessageDataRepository _messageDataRepository;

            Task<ConsumeContext<MessageWithByteArray>> _received;
            byte[] _receivedBytesArray;

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                _messageDataRepository = new InMemoryMessageDataRepository();

                configurator.UseMessageData(_messageDataRepository);

                _received = Handler<MessageWithByteArray>(configurator, async context =>
                {
                    _receivedBytesArray = await context.Message.Bytes.Value;
                });
            }
        }


        [TestFixture]
        public class A_message_with_no_message_data :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_not_add_a_filter()
            {
            }

            IMessageDataRepository _messageDataRepository;

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                _messageDataRepository = new InMemoryMessageDataRepository();

                configurator.UseMessageData(_messageDataRepository);

                Handled<IHaveNoMessageData>(configurator);
            }
        }


        public interface IDocument
        {
            string Title { get; }
            int PageCount { get; }
            MessageData<byte[]> Body { get; }
        }


        class Document :
            IDocument
        {
            public string Title { get; set; }

            public int PageCount { get; set; }

            public MessageData<byte[]> Body { get; set; }
        }


        public interface IMessage
        {
            IDocument Document { get; }
            IDocument[] Documents { get; }
            IList<IDocument> DocumentList { get; }
            IDictionary<string, IDocument> DocumentIndex { get; }
        }


        public class Message : IMessage
        {
            public IDocument Document { get; set; }
            public IDocument[] Documents { get; set; }
            public IList<IDocument> DocumentList { get; set; }
            public IDictionary<string, IDocument> DocumentIndex { get; set; }
        }


        public interface IHaveNoMessageDataEither
        {
            string Value { get; }
            int Name { get; }
        }


        public interface IHaveNoMessageData
        {
            IHaveNoMessageDataEither Child { get; }
            IHaveNoMessageDataEither[] Children { get; }
            IDictionary<string, IHaveNoMessageDataEither> ChildIndex { get; }
        }


        public interface MessageWithByteArray
        {
            MessageData<byte[]> Bytes { get; }
        }


        class MessageWithByteArrayImpl :
            MessageWithByteArray
        {
            public MessageData<byte[]> Bytes { get; set; }
        }


        public interface MessageWithBigData
        {
            MessageData<string> Body { get; }
        }


        class SendMessageWithBigData :
            MessageWithBigData
        {
            public MessageData<string> Body { get; set; }
        }
    }
}
