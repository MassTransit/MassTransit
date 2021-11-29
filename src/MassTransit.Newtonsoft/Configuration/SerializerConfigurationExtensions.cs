namespace MassTransit
{
    using Serialization;


    public static class SerializerConfigurationExtensions
    {
        public static void UseEncryptedSerializer(this IBusFactoryConfigurator configurator, ICryptoStreamProvider provider)
        {
            var factory = new NewtonsoftEncryptedSerializerFactory(provider);

            configurator.AddSerializer(factory);
            configurator.AddDeserializer(factory);
        }

        public static void UseEncryptedSerializer(this IReceiveEndpointConfigurator configurator, ICryptoStreamProvider provider)
        {
            var factory = new NewtonsoftEncryptedSerializerFactory(provider);

            configurator.AddSerializer(factory);
            configurator.AddDeserializer(factory);
        }

        /// <summary>
        /// Serialize messages using the BSON message serializer with AES Encryption
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="provider"></param>
        public static void UseEncryptedSerializerV2(this IBusFactoryConfigurator configurator, ICryptoStreamProviderV2 provider)
        {
            var factory = new NewtonsoftEncryptedV2SerializerFactory(provider);

            configurator.AddSerializer(factory);
            configurator.AddDeserializer(factory);
        }

        /// <summary>
        /// Serialize messages using the BSON message serializer with AES Encryption
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="provider"></param>
        public static void UseEncryptedSerializerV2(this IReceiveEndpointConfigurator configurator, ICryptoStreamProviderV2 provider)
        {
            var factory = new NewtonsoftEncryptedV2SerializerFactory(provider);

            configurator.AddSerializer(factory);
            configurator.AddDeserializer(factory);
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
    }
}
