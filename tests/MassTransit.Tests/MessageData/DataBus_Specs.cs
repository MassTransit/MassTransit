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
        using MassTransit.MessageData.Values;
        using MassTransit.Serialization;
        using NUnit.Framework;
        using TestFramework;


        [TestFixture]
        public class Sending_inlined_message_data :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_be_able_to_write_bytes_too()
            {
                var data = new byte[256];

                var message = new MessageWithByteArrayImpl { Bytes = await _repository.PutBytes(data) };

                await InputQueueSendEndpoint.Send(message);

                ConsumeContext<MessageWithByteArray> receivedBytesContext = await _receivedBytes;

                Assert.Multiple(() =>
                {
                    Assert.That(receivedBytesContext.Message.Bytes.Address, Is.Null);
                    Assert.That(_receivedBytesArray, Is.EqualTo(data));
                });
            }

            [Test]
            public async Task Should_inline_the_string()
            {
                var data = new string('*', 256);

                var message = new SendMessageWithBigData { Body = await _repository.PutString(data) };

                await InputQueueSendEndpoint.Send(message);

                var recievedContext = await _received;
                Assert.Multiple(() =>
                {
                    Assert.That(recievedContext.Message.Body.Address, Is.Null);
                    Assert.That(_receivedBody, Is.EqualTo(data));
                });
            }

            [Test]
            public async Task Should_not_inline_stream()
            {
                var data = new byte[256];
                using var ms = new MemoryStream(data);

                await InputQueueSendEndpoint.Send<MessageWithStream>(new { Stream = ms });

                ConsumeContext<MessageWithStream> streamContext = await _receivedStream;
                Assert.That(streamContext.Message.Stream.Address, Is.Not.Null);

                using var receivedMemoryStream = new MemoryStream();
                await _receivedStreamData.CopyToAsync(receivedMemoryStream);
                Assert.That(receivedMemoryStream.ToArray(), Is.EqualTo(data));
            }

            IMessageDataRepository _repository;
            #pragma warning disable NUnit1032
            Task<ConsumeContext<MessageWithBigData>> _received;
            Task<ConsumeContext<MessageWithByteArray>> _receivedBytes;
            Task<ConsumeContext<MessageWithStream>> _receivedStream;
            #pragma warning restore NUnit1032
            string _receivedBody;
            byte[] _receivedBytesArray;
            Stream _receivedStreamData;

            protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
            {
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                var messageDataPath = Path.Combine(baseDirectory, "MessageData");

                var dataDirectory = new DirectoryInfo(messageDataPath);

                _repository = new FileSystemMessageDataRepository(dataDirectory);
                MessageDataDefaults.AlwaysWriteToRepository = false;
                MessageDataDefaults.Threshold = 4096;
                configurator.UseMessageData(_repository);
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                _received = Handler<MessageWithBigData>(configurator, async context =>
                {
                    _receivedBody = await context.Message.Body.Value;
                });

                _receivedBytes = Handler<MessageWithByteArray>(configurator, async context =>
                {
                    _receivedBytesArray = await context.Message.Bytes.Value;
                });

                _receivedStream = Handler<MessageWithStream>(configurator, async context =>
                {
                    _receivedStreamData = await context.Message.Stream.Value;
                });
            }
        }


        [TestFixture]
        public class Sending_a_large_message_through_the_file_system :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_be_able_to_write_bytes_too()
            {
                var data = new byte[10000];

                var message = new MessageWithByteArrayImpl { Bytes = await _repository.PutBytes(data) };

                await InputQueueSendEndpoint.Send(message);

                await _receivedBytes;

                Assert.That(_receivedBytesArray, Is.EqualTo(data));
            }

            [Test]
            public async Task Should_be_able_to_write_stream_too()
            {
                var data = new byte[10000];
                using MemoryStream ms = new MemoryStream(data);

                var message = new MessageWithStreamImpl { Stream = await _repository.PutStream(ms) };

                await InputQueueSendEndpoint.Send(message);

                await _receivedStream;

                using MemoryStream receivedMemoryStream = new MemoryStream();
                await _receivedStreamData.CopyToAsync(receivedMemoryStream);
                Assert.That(receivedMemoryStream.ToArray(), Is.EqualTo(data));
            }

            [Test]
            public async Task Should_load_the_data_from_the_repository()
            {
                var data = new string('*', 10000);

                var message = new SendMessageWithBigData { Body = await _repository.PutString(data) };

                await InputQueueSendEndpoint.Send(message);

                await _received;

                Assert.That(_receivedBody, Is.EqualTo(data));
            }

            IMessageDataRepository _repository;
            #pragma warning disable NUnit1032
            Task<ConsumeContext<MessageWithBigData>> _received;
            Task<ConsumeContext<MessageWithByteArray>> _receivedBytes;
            Task<ConsumeContext<MessageWithStream>> _receivedStream;
            #pragma warning restore NUnit1032
            string _receivedBody;
            byte[] _receivedBytesArray;
            Stream _receivedStreamData;

            protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
            {
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                var messageDataPath = Path.Combine(baseDirectory, "MessageData");

                var dataDirectory = new DirectoryInfo(messageDataPath);

                _repository = new FileSystemMessageDataRepository(dataDirectory);

                configurator.UseMessageData(_repository);
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                _received = Handler<MessageWithBigData>(configurator, async context =>
                {
                    _receivedBody = await context.Message.Body.Value;
                });

                _receivedBytes = Handler<MessageWithByteArray>(configurator, async context =>
                {
                    _receivedBytesArray = await context.Message.Bytes.Value;
                });

                _receivedStream = Handler<MessageWithStream>(configurator, async context =>
                {
                    _receivedStreamData = await context.Message.Stream.Value;
                });
            }
        }


        [TestFixture]
        public class Sending_a_large_message_through_the_file_system_encrypted :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_be_able_to_write_bytes_too()
            {
                var data = new byte[10000];

                var message = new MessageWithByteArrayImpl { Bytes = await _repository.PutBytes(data) };

                await InputQueueSendEndpoint.Send(message);

                await _receivedBytes;

                Assert.That(_receivedBytesArray, Is.EqualTo(data));
            }

            [Test]
            public async Task Should_be_able_to_write_stream_too()
            {
                var data = new byte[10000];
                using MemoryStream ms = new MemoryStream(data);

                var message = new MessageWithStreamImpl { Stream = await _repository.PutStream(ms) };

                await InputQueueSendEndpoint.Send(message);

                await _receivedStream;

                using MemoryStream receivedMemoryStream = new MemoryStream();
                await _receivedStreamData.CopyToAsync(receivedMemoryStream);
                Assert.That(receivedMemoryStream.ToArray(), Is.EqualTo(data));
            }

            [Test]
            public async Task Should_load_the_data_from_the_repository()
            {
                var data = new string('*', 10000);

                var message = new SendMessageWithBigData { Body = await _repository.PutString(data) };

                await InputQueueSendEndpoint.Send(message);

                await _received;

                Assert.That(_receivedBody, Is.EqualTo(data));
            }

            IMessageDataRepository _repository;
            #pragma warning disable NUnit1032
            Task<ConsumeContext<MessageWithBigData>> _received;
            Task<ConsumeContext<MessageWithByteArray>> _receivedBytes;
            Task<ConsumeContext<MessageWithStream>> _receivedStream;
            #pragma warning restore NUnit1032
            string _receivedBody;
            byte[] _receivedBytesArray;
            Stream _receivedStreamData;

            protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
            {
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

                var messageDataPath = Path.Combine(baseDirectory, "MessageData");

                var dataDirectory = new DirectoryInfo(messageDataPath);

                var fileRepository = new FileSystemMessageDataRepository(dataDirectory);

                ISymmetricKeyProvider keyProvider = new TestSymmetricKeyProvider();
                var cryptoStreamProvider = new AesCryptoStreamProvider(keyProvider, "default");
                _repository = new EncryptedMessageDataRepository(fileRepository, cryptoStreamProvider);

                configurator.UseMessageData(_repository);
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                _received = Handler<MessageWithBigData>(configurator, async context =>
                {
                    _receivedBody = await context.Message.Body.Value;
                });

                _receivedBytes = Handler<MessageWithByteArray>(configurator, async context =>
                {
                    _receivedBytesArray = await context.Message.Bytes.Value;
                });

                _receivedStream = Handler<MessageWithStream>(configurator, async context =>
                {
                    _receivedStreamData = await context.Message.Stream.Value;
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
                var data = NewId.NextGuid().ToString();
                Uri dataAddress;
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data), false))
                {
                    dataAddress = await _messageDataRepository.Put(stream);
                }

                var message = new SendMessageWithBigData { Body = new StoredMessageData<string>(dataAddress, data) };

                await InputQueueSendEndpoint.Send(message);

                await _received;

                Assert.That(_receivedBody, Is.EqualTo(data));
            }

            IMessageDataRepository _messageDataRepository;
            #pragma warning disable NUnit1032
            Task<ConsumeContext<MessageWithBigData>> _received;
            #pragma warning restore NUnit1032
            string _receivedBody;

            protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
            {
                _messageDataRepository = new InMemoryMessageDataRepository();

                configurator.UseMessageData(_messageDataRepository);
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
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
                var data = NewId.NextGuid().ToString();
                Uri dataAddress;
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                using (var stream = new MemoryStream(buffer, false))
                {
                    dataAddress = await _messageDataRepository.Put(stream);
                }

                var message = new Message
                {
                    Document = new Document { Body = new StoredMessageData<byte[]>(dataAddress, buffer) },
                    Documents = new IDocument[]
                    {
                        new Document
                        {
                            Title = "Hello, World",
                            PageCount = 27,
                            Body = new StoredMessageData<byte[]>(dataAddress, buffer)
                        }
                    },
                    DocumentList = new List<IDocument>
                    {
                        new Document
                        {
                            Title = "Hello, World",
                            PageCount = 44,
                            Body = new StoredMessageData<byte[]>(dataAddress, buffer)
                        }
                    },
                    DocumentIndex = new Dictionary<string, IDocument>
                    {
                        { "First", new Document { Body = new StoredMessageData<byte[]>(dataAddress, buffer) } },
                        { "Second", new Document { Body = new StoredMessageData<byte[]>(dataAddress, buffer) } }
                    }
                };

                await InputQueueSendEndpoint.Send(message);

                await _received;

                Assert.Multiple(() =>
                {
                    Assert.That(_body, Is.Not.Null);
                    Assert.That(_body0, Is.Not.Null);
                });

                byte[] result = await _body.Value;
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

            #pragma warning disable NUnit1032
            Task<ConsumeContext<IMessage>> _received;
            #pragma warning restore NUnit1032
            MessageData<byte[]> _body;
            MessageData<byte[]> _body0;
            MessageData<byte[]> _bodyList0;
            MessageData<byte[]> _bodyFirst;
            MessageData<byte[]> _bodySecond;

            protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
            {
                _messageDataRepository = new InMemoryMessageDataRepository();

                configurator.UseMessageData(_messageDataRepository);
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
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
                var nextGuid = NewId.NextGuid();
                var data = nextGuid.ToString();
                Uri dataAddress;
                using (var stream = new MemoryStream(nextGuid.ToByteArray(), false))
                {
                    dataAddress = await _messageDataRepository.Put(stream);
                }

                var message = new MessageWithByteArrayImpl { Bytes = new StoredMessageData<byte[]>(dataAddress, nextGuid.ToByteArray()) };

                await InputQueueSendEndpoint.Send(message);

                await _received;

                var newId = new NewId(_receivedBytesArray);
                Assert.That(newId.ToString(), Is.EqualTo(data));
            }

            IMessageDataRepository _messageDataRepository;

            #pragma warning disable NUnit1032
            Task<ConsumeContext<MessageWithByteArray>> _received;
            #pragma warning restore NUnit1032
            byte[] _receivedBytesArray;

            protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
            {
                _messageDataRepository = new InMemoryMessageDataRepository();

                configurator.UseMessageData(_messageDataRepository);
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                _received = Handler<MessageWithByteArray>(configurator, async context =>
                {
                    _receivedBytesArray = await context.Message.Bytes.Value;
                });
            }
        }


        [TestFixture]
        public class Receiving_a_large_message_with_data_stream :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_load_the_data_from_the_repository()
            {
                var nextGuid = NewId.NextGuid();
                var data = nextGuid.ToString();
                Uri dataAddress;

                using (var stream = new MemoryStream(nextGuid.ToByteArray(), false))
                {
                    dataAddress = await _messageDataRepository.Put(stream);
                }

                using MemoryStream ms = new MemoryStream(nextGuid.ToByteArray());

                var message = new MessageWithStreamImpl { Stream = new StoredMessageData<Stream>(dataAddress, ms) };

                await InputQueueSendEndpoint.Send(message);

                await _received;

                using MemoryStream memoryStreamForReceivedStream = new MemoryStream();
                await _receivedStream.CopyToAsync(memoryStreamForReceivedStream);

                NewId newId = new NewId(memoryStreamForReceivedStream.ToArray());
                Assert.That(newId.ToString(), Is.EqualTo(data));
            }

            IMessageDataRepository _messageDataRepository;

            #pragma warning disable NUnit1032
            Task<ConsumeContext<MessageWithStream>> _received;
            #pragma warning restore NUnit1032
            Stream _receivedStream;

            protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
            {
                _messageDataRepository = new InMemoryMessageDataRepository();

                configurator.UseMessageData(_messageDataRepository);
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                _received = Handler<MessageWithStream>(configurator, async context =>
                {
                    _receivedStream = await context.Message.Stream.Value;
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

            protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
            {
                _messageDataRepository = new InMemoryMessageDataRepository();

                configurator.UseMessageData(_messageDataRepository);
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
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
            IList<Guid> Ids { get; }
            IDocument Document { get; }
            IDocument[] Documents { get; }
            IList<IDocument> DocumentList { get; }
            IDictionary<string, IDocument> DocumentIndex { get; }
        }


        public class Message : IMessage
        {
            public Message()
            {
                Ids = new List<Guid>();
            }

            public IList<Guid> Ids { get; }
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


        public interface MessageWithStream
        {
            MessageData<Stream> Stream { get; }
        }


        class MessageWithStreamImpl : MessageWithStream
        {
            public MessageData<Stream> Stream { get; set; }
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
