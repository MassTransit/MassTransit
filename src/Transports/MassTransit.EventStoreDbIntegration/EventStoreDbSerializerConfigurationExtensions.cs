using System;
using MassTransit.EventStoreDbIntegration;
using MassTransit.Serialization;
using Newtonsoft.Json;

namespace MassTransit
{
    public static class EventStoreDbSerializerConfigurationExtensions
    {
        /// <summary>
        /// Serialize messages using the JSON serializer
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseJsonSerializer(this IEventStoreDbFactoryConfigurator configurator)
        {
            configurator.SetMessageSerializer(() => new JsonMessageSerializer());
        }

        /// <summary>
        /// Configure the serialization settings used to create the message serializer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void ConfigureJsonSerializer(this IEventStoreDbFactoryConfigurator configurator,
            Func<JsonSerializerSettings, JsonSerializerSettings> configure)
        {
            JsonMessageSerializer.SerializerSettings = configure(JsonMessageSerializer.SerializerSettings);
        }

        /// <summary>
        /// Configure the serialization settings used to create the message deserializer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void ConfigureJsonDeserializer(this IEventStoreDbFactoryConfigurator configurator,
            Func<JsonSerializerSettings, JsonSerializerSettings> configure)
        {
            JsonMessageSerializer.DeserializerSettings = configure(JsonMessageSerializer.DeserializerSettings);
        }

        /// <summary>
        /// Configure the serialization settings used to create the BSON message serializer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void ConfigureBsonSerializer(this IEventStoreDbFactoryConfigurator configurator,
            Func<JsonSerializerSettings, JsonSerializerSettings> configure)
        {
            BsonMessageSerializer.SerializerSettings = configure(BsonMessageSerializer.SerializerSettings);
        }

        /// <summary>
        /// Configure the serialization settings used to create the BSON message deserializer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void ConfigureBsonDeserializer(this IEventStoreDbFactoryConfigurator configurator,
            Func<JsonSerializerSettings, JsonSerializerSettings> configure)
        {
            BsonMessageSerializer.DeserializerSettings = configure(BsonMessageSerializer.DeserializerSettings);
        }

        /// <summary>
        /// Serialize messages using the BSON message serializer
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseBsonSerializer(this IEventStoreDbFactoryConfigurator configurator)
        {
            configurator.SetMessageSerializer(() => new BsonMessageSerializer());
        }

        /// <summary>
        /// Serialize messages using the raw JSON message serializer
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseRawJsonSerializer(this IEventStoreDbFactoryConfigurator configurator)
        {
            configurator.SetMessageSerializer(() => new RawJsonMessageSerializer());
        }

        public static void UseEncryptedSerializer(this IEventStoreDbFactoryConfigurator configurator, ICryptoStreamProvider streamProvider)
        {
            configurator.SetMessageSerializer(() => new EncryptedMessageSerializer(streamProvider));
        }

        /// <summary>
        /// Serialize messages using the BSON message serializer with AES Encryption
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="symmetricKey">
        /// Cryptographic key for both encryption of plaintext message and decryption of ciphertext message
        /// </param>
        public static void UseEncryption(this IEventStoreDbFactoryConfigurator configurator, byte[] symmetricKey)
        {
            var keyProvider = new ConstantSecureKeyProvider(symmetricKey);
            var streamProvider = new AesCryptoStreamProviderV2(keyProvider);

            configurator.UseEncryptedSerializerV2(streamProvider);
        }

        /// <summary>
        /// Serialize messages using the BSON message serializer with AES Encryption
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="keyProvider">
        /// The custom key provider to provide the symmetric key for encryption of plaintext message and decryption of ciphertext message
        /// </param>
        public static void UseEncryption(this IEventStoreDbFactoryConfigurator configurator, ISecureKeyProvider keyProvider)
        {
            var streamProvider = new AesCryptoStreamProviderV2(keyProvider);

            configurator.UseEncryptedSerializerV2(streamProvider);
        }

        /// <summary>
        /// Serialize messages using the BSON message serializer with AES Encryption
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="streamProvider"></param>
        public static void UseEncryptedSerializerV2(this IEventStoreDbFactoryConfigurator configurator, ICryptoStreamProviderV2 streamProvider)
        {
            configurator.SetMessageSerializer(() => new EncryptedMessageSerializerV2(streamProvider));
        }

        /// <summary>
        /// Serialize messages using the XML message serializer
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseXmlSerializer(this IEventStoreDbFactoryConfigurator configurator)
        {
            configurator.SetMessageSerializer(() => new XmlMessageSerializer());
        }
    }
}
