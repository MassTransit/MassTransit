// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
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
        public static void ConfigureJsonDeserializer(this IBusFactoryConfigurator configurator,
            Func<JsonSerializerSettings, JsonSerializerSettings> configure)
        {
            JsonMessageSerializer.DeserializerSettings = configure(JsonMessageSerializer.DeserializerSettings);
        }

         ///<summary>
        /// Configure the serialization settings used to create the BSON message serializer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
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
        /// <param name="symmetricKey">Cryptographic key for both encryption of plaintext message and decryption of ciphertext message</param>
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
        /// <param name="symmetricKey">Cryptographic key for both encryption of plaintext message and decryption of ciphertext message</param>
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
        /// <param name="keyProvider">The custom key provider to provide the symmetric key for encryption of plaintext message and decryption of ciphertext message</param>
        public static void UseEncryption(this IBusFactoryConfigurator configurator, ISecureKeyProvider keyProvider)
        {
            var streamProvider = new AesCryptoStreamProviderV2(keyProvider);

            configurator.UseEncryptedSerializerV2(streamProvider);
        }

        /// <summary>
        /// Serialize messages using the BSON message serializer with AES Encryption
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="keyProvider">The custom key provider to provide the symmetric key for encryption of plaintext message and decryption of ciphertext message</param>
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
        

    #if !NETCORE
        /// <summary>
        /// Serialize message using the .NET binary formatter (also adds support for the binary deserializer)
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseBinarySerializer(this IBusFactoryConfigurator configurator)
        {
            configurator.SetMessageSerializer(() => new BinaryMessageSerializer());

            configurator.SupportBinaryMessageDeserializer();
        }

        /// <summary>
        /// Serialize message using the .NET binary formatter (also adds support for the binary deserializer)
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseBinarySerializer(this IReceiveEndpointConfigurator configurator)
        {
            configurator.SetMessageSerializer(() => new BinaryMessageSerializer());

            configurator.SupportBinaryMessageDeserializer();
        }

        /// <summary>
        /// Add support for the binary message deserializer to the bus. This serializer is not supported
        /// by default.
        /// </summary>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public static void SupportBinaryMessageDeserializer(this IBusFactoryConfigurator configurator)
        {
            configurator.AddMessageDeserializer(BinaryMessageSerializer.BinaryContentType, () => new BinaryMessageDeserializer());
        }

        /// <summary>
        /// Add support for the binary message deserializer to the bus. This serializer is not supported
        /// by default.
        /// </summary>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public static void SupportBinaryMessageDeserializer(this IReceiveEndpointConfigurator configurator)
        {
            configurator.AddMessageDeserializer(BinaryMessageSerializer.BinaryContentType, () => new BinaryMessageDeserializer());
        }
    #endif
    }
}
