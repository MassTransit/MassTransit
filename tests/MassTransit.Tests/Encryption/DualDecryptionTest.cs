#nullable enable
namespace MassTransit.Tests.Encryption
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Cryptography;
    using InMemoryTransport;
    using MassTransit.Serialization;
    using NUnit.Framework;


    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class DualDecryptionTest
    {
        [Test]
        public void MessageEncryptionTest_HavingMessageIsEncrypted_WhenMessageIsReceived_MessageIsDecryptedWithPrimairyKey()
        {
            // Arrange
            var keys = GenerateEncryptionKeys();

            var factory = CreateSerializerFactory(keys);

            var serializer = factory.CreateSerializer();
            var deserializer = factory.CreateDeserializer();

            var inputMessage = CreateTestMessage(out var inputTestEvent);

            // Act
            var inMemorySendContext = new InMemorySendContext<TestMessage>(inputTestEvent);
            var messageBody = serializer.GetMessageBody(inMemorySendContext);
            var encryptedMessage = messageBody.GetString();

            var deserializerContext = deserializer.Deserialize(messageBody, EmptyHeaders.Instance);
            var canDecryptMessage = deserializerContext.TryGetMessage(out TestMessage? decryptedReceivedTestEvent);

            // Assert
            Assert.AreNotEqual(inputMessage, encryptedMessage);
            Assert.IsTrue(canDecryptMessage);
            Assert.AreEqual(inputTestEvent, decryptedReceivedTestEvent);
        }

        [Test]
        public void MessageEncryptionTest_HavingMessageIsEncrypted_WhenMessageIsReceived_MessageIsDecryptedWithSecondaryKey()
        {
            // Arrange
            var firstServiceKeyset = GenerateEncryptionKeys();

            // second factory, representing another service.
            var keyNotInUse = GenerateEncryptionKeys().Item1;
            var secondServiceKeySet = new Tuple<string, string>(keyNotInUse, firstServiceKeyset.Item1);

            var firstFactory = CreateSerializerFactory(firstServiceKeyset);
            var firstSerializer = firstFactory.CreateSerializer();

            var secondFactory = CreateSerializerFactory(secondServiceKeySet);
            var secondDeserializer = secondFactory.CreateDeserializer();

            var inputMessage = CreateTestMessage(out var inputTestEvent);

            // Act
            var inMemorySendContext = new InMemorySendContext<TestMessage>(inputTestEvent);
            var messageBody = firstSerializer.GetMessageBody(inMemorySendContext);
            var encryptedMessage = messageBody.GetString();

            var deserializerContext = secondDeserializer.Deserialize(messageBody, EmptyHeaders.Instance);
            var canDecryptMessage = deserializerContext.TryGetMessage(out TestMessage? decryptedReceivedTestEvent);

            // Assert
            Assert.AreNotEqual(inputMessage, encryptedMessage);
            Assert.IsTrue(canDecryptMessage);
            Assert.AreEqual(inputTestEvent, decryptedReceivedTestEvent);
        }
        
        [Test]
        public void MessageEncryptionTest_HavingMessageIsEncrypted_WhenMessageIsReceived_MessageCannotBeDecryptedWithDifferentKeyset()
        {
            // Arrange
            var firstServiceKeyset = GenerateEncryptionKeys();

            // second factory, representing another service.
            var secondServiceKeySet = GenerateEncryptionKeys();

            var firstFactory = CreateSerializerFactory(firstServiceKeyset);
            var firstSerializer = firstFactory.CreateSerializer();

            var secondFactory = CreateSerializerFactory(secondServiceKeySet);
            var secondDeserializer = secondFactory.CreateDeserializer();

            var inputMessage = CreateTestMessage(out var inputTestEvent);

            // Act
            var inMemorySendContext = new InMemorySendContext<TestMessage>(inputTestEvent);
            var messageBody = firstSerializer.GetMessageBody(inMemorySendContext);
            var encryptedMessage = messageBody.GetString();

            void DeserializeAction() => secondDeserializer.Deserialize(messageBody, EmptyHeaders.Instance);

            // Assert

            Assert.AreNotEqual(inputMessage, encryptedMessage);
            Assert.Throws<SerializationException>(DeserializeAction);
        }

        static string CreateTestMessage(out TestMessage inputTestEvent)
        {
            var inputMessage = "test message";
            inputTestEvent = new TestMessage(Guid.NewGuid(), inputMessage);

            return inputMessage;
        }

        static DualNewtonsoftDecryptionV2SerializerFactory CreateSerializerFactory(Tuple<string, string> keys)
        {
            var primaryKey = Convert.FromBase64String(keys.Item1);
            var keyProvider = new ConstantSecureKeyProvider(primaryKey);
            var primaryCrypto = new AesCryptoStreamProviderV2(keyProvider);

            var secondaryKey = Convert.FromBase64String(keys.Item2);
            var keyProvider2 = new ConstantSecureKeyProvider(secondaryKey);
            var secondaryCrypto = new AesCryptoStreamProviderV2(keyProvider2);

            return new DualNewtonsoftDecryptionV2SerializerFactory(primaryCrypto, secondaryCrypto);
        }

        static Tuple<string, string> GenerateEncryptionKeys()
        {
            var aes = Aes.Create();
            aes.GenerateKey();

            var key1 = Convert.ToBase64String(aes.Key);

            aes.GenerateKey();
            var key2 = Convert.ToBase64String(aes.Key);

            return new Tuple<string, string>(key1, key2);
        }
        
    }
}
