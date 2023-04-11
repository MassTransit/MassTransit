namespace MassTransit.Serialization
{
    using MassTransit;

    public static class DualDecryptionExtensions
    {
        /// <summary>
        /// Serialize messages using the BSON message serializer with AES Encryption
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="primaryProvider"></param>
        /// <param name="secondaryProvider"></param>
        public static void UseDualDecryptionWithNewtonsoftJson(this IBusFactoryConfigurator configurator, ICryptoStreamProviderV2 primaryProvider, ICryptoStreamProviderV2 secondaryProvider)
        {
            var factory = new DualNewtonsoftDecryptionV2SerializerFactory(primaryProvider, secondaryProvider);

            configurator.AddSerializer(factory);
            configurator.AddDeserializer(factory);
        }

        /// <summary>
        /// Serialize messages using the BSON message serializer with AES Encryption
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="primaryProvider"></param>
        /// <param name="secondaryProvider"></param>
        public static void UseDualDecryptionWithNewtonsoftJson(this IReceiveEndpointConfigurator configurator, ICryptoStreamProviderV2 primaryProvider, ICryptoStreamProviderV2 secondaryProvider)
        {
            var factory = new DualNewtonsoftDecryptionV2SerializerFactory(primaryProvider, secondaryProvider);

            configurator.AddSerializer(factory);
            configurator.AddDeserializer(factory);
        }

        /// <summary>
        /// Serialize messages using the BSON message serializer with AES Encryption
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="primarySymmetricKey">
        /// Cryptographic key for both encryption of plaintext message and decryption of ciphertext message
        /// </param>
        /// <param name="secondarySymmetricKey">
        /// Cryptographic key for decryption of ciphertext message if the primary key fails
        /// </param>
        public static void UseDualDecryptionWithNewtonsoftJson(this IBusFactoryConfigurator configurator, byte[] primarySymmetricKey, byte[] secondarySymmetricKey)
        {
            var primaryKeyProvider = new ConstantSecureKeyProvider(primarySymmetricKey);
            var secondaryKeyProvider = new ConstantSecureKeyProvider(secondarySymmetricKey);

            configurator.UseDualDecryptionWithNewtonsoftJson(primaryKeyProvider, secondaryKeyProvider);
        }

        /// <summary>
        /// Serialize messages using the BSON message serializer with AES Encryption
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="primarySymmetricKey">
        /// Cryptographic key for both encryption of plaintext message and decryption of ciphertext message
        /// </param>
        /// <param name="secondarySymmetricKey">
        /// Cryptographic key for decryption of ciphertext message if the primary key fails
        /// </param>
        public static void UseDualDecryptionWithNewtonsoftJson(this IReceiveEndpointConfigurator configurator, byte[] primarySymmetricKey, byte[] secondarySymmetricKey)
        {
            var primaryKeyProvider = new ConstantSecureKeyProvider(primarySymmetricKey);
            var secondaryKeyProvider = new ConstantSecureKeyProvider(secondarySymmetricKey);

            configurator.UseDualDecryptionWithNewtonsoftJson(primaryKeyProvider, secondaryKeyProvider);
        }

        /// <summary>
        /// Serialize messages using the BSON message serializer with AES Encryption
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="primaryKeyProvider">
        /// The custom key provider to provide the symmetric key for encryption of plaintext message and decryption of ciphertext message
        /// </param>
        /// <param name="secondaryKeyProvider">
        /// The custom key provider to provide the symmetric key for decryption of ciphertext message if the primary key fails
        /// </param>
        public static void UseDualDecryptionWithNewtonsoftJson(this IBusFactoryConfigurator configurator, ISecureKeyProvider primaryKeyProvider, ISecureKeyProvider secondaryKeyProvider)
        {
            var primaryProvider = new AesCryptoStreamProviderV2(primaryKeyProvider);
            var secondaryProvider = new AesCryptoStreamProviderV2(secondaryKeyProvider);

            configurator.UseDualDecryptionWithNewtonsoftJson(primaryProvider, secondaryProvider);
        }

        /// <summary>
        /// Serialize messages using the BSON message serializer with AES Encryption
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="primaryKeyProvider">
        /// The custom key provider to provide the symmetric key for encryption of plaintext message and decryption of ciphertext message
        /// </param>
        /// <param name="secondaryKeyProvider">
        /// The custom key provider to provide the symmetric key for decryption of ciphertext message if the primary key fails
        /// </param>
        public static void UseDualDecryptionWithNewtonsoftJson(this IReceiveEndpointConfigurator configurator, ISecureKeyProvider primaryKeyProvider, ISecureKeyProvider secondaryKeyProvider)
        {
            var primaryProvider = new AesCryptoStreamProviderV2(primaryKeyProvider);
            var secondaryProvider = new AesCryptoStreamProviderV2(secondaryKeyProvider);

            configurator.UseDualDecryptionWithNewtonsoftJson(primaryProvider, secondaryProvider);
        }
    }
}
