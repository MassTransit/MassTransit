namespace MassTransit
{
    using System;
    using Newtonsoft.Json;
    using Serialization;


    public static class SerializerConfigurationExtensions
    {
        /// <summary>
        /// Serialize messages using the JSON serializer
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseJsonSerializer(this IBusFactoryConfigurator configurator)
        {
            configurator.SetMessageSerializer(() => new JsonMessageSerializer());
        }

        /// <summary>
        /// Serialize messages using the raw xml message serializer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="options">If false, only supported messages types (read from the message headers) will be returned</param>
        public static void UseRawXmlSerialzier(this IBusFactoryConfigurator configurator, RawXmlSerializerOptions options)
        {
            configurator.SetMessageSerializer(() => new XmlRawSerializer());

            configurator.AddMessageDeserializer(XmlRawSerializer.XmlContentType,
                () => new RawXmlMessageDeserializer(XmlRawSerializer.XmlSerializer, options));
        }

        /// <summary>
        /// Serialize messages using the JSON serializer
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseJsonSerializer(this IReceiveEndpointConfigurator configurator)
        {
            configurator.SetMessageSerializer(() => new JsonMessageSerializer());
        }

        /// <summary>
        /// Configure the serialization settings used to create the message serializer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <remarks>
        /// These settings are applied globally to <see cref="JsonMessageSerializer.SerializerSettings"/>.
        /// </remarks>
        public static void ConfigureJsonSerializer(this IBusFactoryConfigurator configurator,
            Func<JsonSerializerSettings, JsonSerializerSettings> configure)
        {
            JsonMessageSerializer.SerializerSettings = configure(JsonMessageSerializer.SerializerSettings);
        }

        /// <summary>
        /// Configure the serialization settings used to create the message deserializer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <remarks>
        /// These settings are applied globally to <see cref="JsonMessageSerializer.SerializerSettings"/>.
        /// </remarks>
        public static void ConfigureJsonDeserializer(this IBusFactoryConfigurator configurator,
            Func<JsonSerializerSettings, JsonSerializerSettings> configure)
        {
            JsonMessageSerializer.DeserializerSettings = configure(JsonMessageSerializer.DeserializerSettings);
        }

        /// <summary>
        /// Configure the serialization settings used to create the BSON message serializer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <remarks>
        /// These settings are applied globally to <see cref="JsonMessageSerializer.SerializerSettings"/>.
        /// </remarks>
        public static void ConfigureBsonSerializer(this IBusFactoryConfigurator configurator,
            Func<JsonSerializerSettings, JsonSerializerSettings> configure)
        {
            BsonMessageSerializer.SerializerSettings = configure(BsonMessageSerializer.SerializerSettings);
        }

        /// <summary>
        /// Configure the serialization settings used to create the BSON message deserializer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <remarks>
        /// These settings are applied globally to <see cref="JsonMessageSerializer.SerializerSettings"/>.
        /// </remarks>
        public static void ConfigureBsonDeserializer(this IBusFactoryConfigurator configurator,
            Func<JsonSerializerSettings, JsonSerializerSettings> configure)
        {
            BsonMessageSerializer.DeserializerSettings = configure(BsonMessageSerializer.DeserializerSettings);
        }

        /// <summary>
        /// Serialize messages using the BSON message serializer
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseBsonSerializer(this IBusFactoryConfigurator configurator)
        {
            configurator.SetMessageSerializer(() => new BsonMessageSerializer());
        }

        /// <summary>
        /// Serialize messages using the BSON message serializer
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseBsonSerializer(this IReceiveEndpointConfigurator configurator)
        {
            configurator.SetMessageSerializer(() => new BsonMessageSerializer());
        }

        /// <summary>
        /// Serialize messages using the raw JSON message serializer
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseRawJsonSerializer(this IBusFactoryConfigurator configurator)
        {
            configurator.SetMessageSerializer(() => new RawJsonMessageSerializer());

            configurator.AddMessageDeserializer(RawJsonMessageSerializer.RawJsonContentType,
                () => new RawJsonMessageDeserializer(JsonMessageSerializer.Deserializer));
        }

        /// <summary>
        /// Serialize messages using the raw JSON message serializer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="options">If false, only supported messages types (read from the message headers) will be returned</param>
        public static void UseRawJsonSerializer(this IBusFactoryConfigurator configurator, RawJsonSerializerOptions options)
        {
            configurator.SetMessageSerializer(() => new RawJsonMessageSerializer());

            configurator.AddMessageDeserializer(RawJsonMessageSerializer.RawJsonContentType,
                () => new RawJsonMessageDeserializer(JsonMessageSerializer.Deserializer, options));
        }

        /// <summary>
        /// Serialize messages using the raw JSON message serializer
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseRawJsonSerializer(this IReceiveEndpointConfigurator configurator)
        {
            configurator.SetMessageSerializer(() => new RawJsonMessageSerializer());

            configurator.AddMessageDeserializer(RawJsonMessageSerializer.RawJsonContentType,
                () => new RawJsonMessageDeserializer(JsonMessageSerializer.Deserializer));
        }

        /// <summary>
        /// Serialize messages using the raw JSON message serializer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="options">If false, only supported messages types (read from the message headers) will be returned</param>
        public static void UseRawJsonSerializer(this IReceiveEndpointConfigurator configurator, RawJsonSerializerOptions options)
        {
            configurator.SetMessageSerializer(() => new RawJsonMessageSerializer());

            configurator.AddMessageDeserializer(RawJsonMessageSerializer.RawJsonContentType,
                () => new RawJsonMessageDeserializer(JsonMessageSerializer.Deserializer, options));
        }

        public static void UseEncryptedSerializer(this IBusFactoryConfigurator configurator, ICryptoStreamProvider streamProvider)
        {
            configurator.SetMessageSerializer(() => new EncryptedMessageSerializer(streamProvider));

            configurator.AddMessageDeserializer(EncryptedMessageSerializer.EncryptedContentType,
                () => new EncryptedMessageDeserializer(BsonMessageSerializer.Deserializer, streamProvider));
        }

        public static void UseEncryptedSerializer(this IReceiveEndpointConfigurator configurator, ICryptoStreamProvider streamProvider)
        {
            configurator.SetMessageSerializer(() => new EncryptedMessageSerializer(streamProvider));

            configurator.AddMessageDeserializer(EncryptedMessageSerializer.EncryptedContentType,
                () => new EncryptedMessageDeserializer(BsonMessageSerializer.Deserializer, streamProvider));
        }

        /// <summary>
        /// Serialize messages using the BSON message serializer with AES Encryption
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="symmetricKey">
        /// Cryptographic key for both encryption of plaintext message and decryption of ciphertext message
        /// </param>
        public static void UseEncryption(this IBusFactoryConfigurator configurator, byte[] symmetricKey)
        {
            var keyProvider = new ConstantSecureKeyProvider(symmetricKey);
            var streamProvider = new AesCryptoStreamProviderV2(keyProvider);

            configurator.UseEncryptedSerializerV2(streamProvider);
        }

        /// <summary>
        /// Serialize messages using the BSON message serializer with AES Encryption
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="symmetricKey">
        /// Cryptographic key for both encryption of plaintext message and decryption of ciphertext message
        /// </param>
        public static void UseEncryption(this IReceiveEndpointConfigurator configurator, byte[] symmetricKey)
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
        public static void UseEncryption(this IBusFactoryConfigurator configurator, ISecureKeyProvider keyProvider)
        {
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
        public static void UseEncryption(this IReceiveEndpointConfigurator configurator, ISecureKeyProvider keyProvider)
        {
            var streamProvider = new AesCryptoStreamProviderV2(keyProvider);

            configurator.UseEncryptedSerializerV2(streamProvider);
        }

        /// <summary>
        /// Serialize messages using the BSON message serializer with AES Encryption
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="streamProvider"></param>
        public static void UseEncryptedSerializerV2(this IBusFactoryConfigurator configurator, ICryptoStreamProviderV2 streamProvider)
        {
            configurator.SetMessageSerializer(() => new EncryptedMessageSerializerV2(streamProvider));

            configurator.AddMessageDeserializer(EncryptedMessageSerializerV2.EncryptedContentType,
                () => new EncryptedMessageDeserializerV2(BsonMessageSerializer.Deserializer, streamProvider));
        }

        /// <summary>
        /// Serialize messages using the BSON message serializer with AES Encryption
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="streamProvider"></param>
        public static void UseEncryptedSerializerV2(this IReceiveEndpointConfigurator configurator, ICryptoStreamProviderV2 streamProvider)
        {
            configurator.SetMessageSerializer(() => new EncryptedMessageSerializerV2(streamProvider));

            configurator.AddMessageDeserializer(EncryptedMessageSerializerV2.EncryptedContentType,
                () => new EncryptedMessageDeserializerV2(BsonMessageSerializer.Deserializer, streamProvider));
        }

        /// <summary>
        /// Serialize messages using the XML message serializer
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseXmlSerializer(this IBusFactoryConfigurator configurator)
        {
            configurator.SetMessageSerializer(() => new XmlMessageSerializer());
        }

        /// <summary>
        /// Serialize messages using the XML message serializer
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseXmlSerializer(this IReceiveEndpointConfigurator configurator)
        {
            configurator.SetMessageSerializer(() => new XmlMessageSerializer());
        }
    }
}
