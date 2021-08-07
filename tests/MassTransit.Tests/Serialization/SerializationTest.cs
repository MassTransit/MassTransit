namespace MassTransit.Tests.Serialization
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using Context;
    using MassTransit.Serialization;
    using MassTransit.Transports.InMemory.Contexts;
    using MassTransit.Transports.InMemory.Fabric;
    using Metadata;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;


    public abstract class SerializationTest :
        InMemoryTestFixture
    {
        readonly Uri _destinationAddress = new Uri("loopback://localhost/destination");
        readonly Uri _faultAddress = new Uri("loopback://localhost/fault");
        protected readonly Guid _requestId = Guid.NewGuid();
        readonly Uri _responseAddress = new Uri("loopback://localhost/response");
        readonly Type _serializerType;
        readonly Uri _sourceAddress = new Uri("loopback://localhost/source");
        protected IMessageDeserializer Deserializer;
        protected IMessageSerializer Serializer;

        public SerializationTest(Type serializerType)
        {
            _serializerType = serializerType;
        }

        [OneTimeSetUp]
        public void SetupSerializationTest()
        {
            if (_serializerType == typeof(JsonMessageSerializer))
            {
                Serializer = new JsonMessageSerializer();
                Deserializer = new JsonMessageDeserializer(JsonMessageSerializer.Deserializer);
            }
            else if (_serializerType == typeof(SystemTextJsonMessageSerializer))
            {
                Serializer = new SystemTextJsonMessageSerializer();
                Deserializer = new SystemTextJsonMessageDeserializer();
            }
            else if (_serializerType == typeof(BsonMessageSerializer))
            {
                Serializer = new BsonMessageSerializer();
                Deserializer = new BsonMessageDeserializer(BsonMessageSerializer.Deserializer);
            }
            else if (_serializerType == typeof(XmlMessageSerializer))
            {
                Serializer = new XmlMessageSerializer();
                Deserializer = new XmlMessageDeserializer(XmlJsonMessageSerializer.Deserializer);
            }
            else if (_serializerType == typeof(EncryptedMessageSerializer))
            {
                ISymmetricKeyProvider keyProvider = new TestSymmetricKeyProvider();
                var streamProvider = new AesCryptoStreamProvider(keyProvider, "default");

                Serializer = new EncryptedMessageSerializer(streamProvider);
                Deserializer = new EncryptedMessageDeserializer(BsonMessageSerializer.Deserializer, streamProvider);
            }
            else if (_serializerType == typeof(EncryptedMessageSerializerV2))
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

                Serializer = new EncryptedMessageSerializerV2(streamProvider);
                Deserializer = new EncryptedMessageDeserializerV2(BsonMessageSerializer.Deserializer, streamProvider);
            }
            else
                throw new ArgumentException("The serializer type is unknown");
        }

        protected T SerializeAndReturn<T>(T obj)
            where T : class
        {
            byte[] serializedMessageData = Serialize(obj);

            return Return<T>(serializedMessageData);
        }

        protected byte[] Serialize<T>(T obj)
            where T : class
        {
            using (var output = new MemoryStream())
            {
                var sendContext = new MessageSendContext<T>(obj);

                sendContext.SourceAddress = _sourceAddress;
                sendContext.DestinationAddress = _destinationAddress;
                sendContext.FaultAddress = _faultAddress;
                sendContext.ResponseAddress = _responseAddress;
                sendContext.RequestId = _requestId;


                Serializer.Serialize(output, sendContext);

                byte[] serializedMessageData = output.ToArray();

                Trace.WriteLine(Encoding.UTF8.GetString(serializedMessageData));
                return serializedMessageData;
            }
        }

        protected T Return<T>(byte[] serializedMessageData)
            where T : class
        {
            var message = new InMemoryTransportMessage(Guid.NewGuid(), serializedMessageData, Serializer.ContentType.MediaType, TypeMetadataCache<T>.ShortName);
            var receiveContext = new InMemoryReceiveContext(message, TestConsumeContext.GetContext());

            var consumeContext = Deserializer.Deserialize(receiveContext);

            consumeContext.TryGetMessage(out ConsumeContext<T> messageContext);

            messageContext.ShouldNotBe(null);

            messageContext.SourceAddress.ShouldBe(_sourceAddress);
            messageContext.DestinationAddress.ShouldBe(_destinationAddress);
            messageContext.FaultAddress.ShouldBe(_faultAddress);
            messageContext.ResponseAddress.ShouldBe(_responseAddress);
            messageContext.RequestId.HasValue.ShouldBe(true);
            messageContext.RequestId.Value.ShouldBe(_requestId);

            return messageContext.Message;
        }

        protected virtual void TestSerialization<T>(T message)
            where T : class
        {
            var result = SerializeAndReturn(message);

            message.Equals(result).ShouldBe(true);
        }
    }
}
