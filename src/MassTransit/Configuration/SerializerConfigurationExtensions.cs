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
    using EndpointSpecifications;
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
            configurator.AddBusFactorySpecification(new SetMessageSerializerBusFactorySpecification<JsonMessageSerializer>());
        }

        /// <summary>
        /// Serialize messages using the JSON serializer
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseJsonSerializer(this IReceiveEndpointConfigurator configurator)
        {
            configurator.AddEndpointSpecification(new SetMessageSerializerReceiveEndpointSpecification<JsonMessageSerializer>());
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

        /// <summary>
        /// Serialize messages using the BSON message serializer
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseBsonSerializer(this IBusFactoryConfigurator configurator)
        {
            configurator.AddBusFactorySpecification(new SetMessageSerializerBusFactorySpecification<BsonMessageSerializer>());
        }

        /// <summary>
        /// Serialize messages using the BSON message serializer
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseBsonSerializer(this IReceiveEndpointConfigurator configurator)
        {
            configurator.AddEndpointSpecification(new SetMessageSerializerReceiveEndpointSpecification<BsonMessageSerializer>());
        }

        public static void UseEncryptedSerializer(this IBusFactoryConfigurator configurator, ICryptoStreamProvider streamProvider)
        {
            configurator.AddBusFactorySpecification(new EncryptedMessageSerializerBusFactorySpecification(streamProvider));
        }

        public static void UseEncryptedSerializer(this IReceiveEndpointConfigurator configurator, ICryptoStreamProvider streamProvider)
        {
            configurator.AddEndpointSpecification(new EncryptedMessageSerializerReceiveEndpointSpecification(streamProvider));
        }

        /// <summary>
        /// Serialize messages using the XML message serializer
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseXmlSerializer(this IBusFactoryConfigurator configurator)
        {
            configurator.AddBusFactorySpecification(new SetMessageSerializerBusFactorySpecification<XmlMessageSerializer>());
        }

        /// <summary>
        /// Serialize messages using the XML message serializer
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseXmlSerializer(this IReceiveEndpointConfigurator configurator)
        {
            configurator.AddEndpointSpecification(new SetMessageSerializerReceiveEndpointSpecification<XmlMessageSerializer>());
        }

#if !NETCORE
        /// <summary>
        /// Serialize message using the .NET binary formatter (also adds support for the binary deserializer)
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseBinarySerializer(this IBusFactoryConfigurator configurator)
        {
            configurator.AddBusFactorySpecification(new SetMessageSerializerBusFactorySpecification<BinaryMessageSerializer>());

            configurator.SupportBinaryMessageDeserializer();
        }

        /// <summary>
        /// Serialize message using the .NET binary formatter (also adds support for the binary deserializer)
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseBinarySerializer(this IReceiveEndpointConfigurator configurator)
        {
            configurator.AddEndpointSpecification(new SetMessageSerializerReceiveEndpointSpecification<BinaryMessageSerializer>());

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
            configurator.AddBusFactorySpecification(new SupportMessageDeserializerBusFactorySpecification(BinaryMessageSerializer.BinaryContentType,
                () => new BinaryMessageDeserializer()));
        }

        /// <summary>
        /// Add support for the binary message deserializer to the bus. This serializer is not supported
        /// by default.
        /// </summary>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public static void SupportBinaryMessageDeserializer(this IReceiveEndpointConfigurator configurator)
        {
            configurator.AddEndpointSpecification(new SupportMessageDeserializerReceiveEndpointSpecification(BinaryMessageSerializer.BinaryContentType,
                () => new BinaryMessageDeserializer()));
        }
#endif
    }
}