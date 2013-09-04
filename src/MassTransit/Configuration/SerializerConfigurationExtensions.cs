// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using BusConfigurators;
    using EndpointConfigurators;
    using Magnum.Reflection;
    using Newtonsoft.Json;
    using Serialization;


    public static class SerializerConfigurationExtensions
    {
        public static T UseJsonSerializer<T>(this T configurator)
            where T : EndpointFactoryConfigurator
        {
            configurator.SetDefaultSerializer<JsonMessageSerializer>();

            return configurator;
        }

        public static T ConfigureJsonSerializer<T>(this T configurator,
            Func<JsonSerializerSettings, JsonSerializerSettings> configure)
            where T : EndpointFactoryConfigurator
        {
            JsonMessageSerializer.SerializerSettings = configure(JsonMessageSerializer.SerializerSettings);

            return configurator;
        }

        public static T ConfigureJsonDeserializer<T>(this T configurator,
            Func<JsonSerializerSettings, JsonSerializerSettings> configure)
            where T : EndpointFactoryConfigurator
        {
            JsonMessageSerializer.DeserializerSettings = configure(JsonMessageSerializer.DeserializerSettings);

            return configurator;
        }

        public static T UseBsonSerializer<T>(this T configurator)
            where T : EndpointFactoryConfigurator
        {
            configurator.SetDefaultSerializer<BsonMessageSerializer>();

            return configurator;
        }

        public static T UseXmlSerializer<T>(this T configurator)
            where T : EndpointFactoryConfigurator
        {
            configurator.SetDefaultSerializer<XmlMessageSerializer>();

            return configurator;
        }

        public static T UseVersionOneXmlSerializer<T>(this T configurator)
            where T : EndpointFactoryConfigurator
        {
            configurator.SetDefaultSerializer<VersionOneXmlMessageSerializer>();

            return configurator;
        }

        public static T UseBinarySerializer<T>(this T configurator)
            where T : EndpointFactoryConfigurator
        {
            configurator.SetDefaultSerializer<BinaryMessageSerializer>();

            return configurator;
        }

        /// <summary>
        /// Support the receipt of messages serialized by the JsonMessageSerializer
        /// </summary>
        public static T SupportJsonSerializer<T>(this T configurator)
            where T : EndpointFactoryConfigurator
        {
            configurator.SupportMessageSerializer<JsonMessageSerializer>();

            return configurator;
        }

        public static T SupportBsonSerializer<T>(this T configurator)
            where T : EndpointFactoryConfigurator
        {
            configurator.SupportMessageSerializer<BsonMessageSerializer>();

            return configurator;
        }

        public static T SupportXmlSerializer<T>(this T configurator)
            where T : EndpointFactoryConfigurator
        {
            configurator.SupportMessageSerializer<XmlMessageSerializer>();

            return configurator;
        }

        public static T SupportVersionOneXmlSerializer<T>(this T configurator)
            where T : EndpointFactoryConfigurator
        {
            configurator.SupportMessageSerializer<VersionOneXmlMessageSerializer>();

            return configurator;
        }

        public static T SupportBinarySerializer<T>(this T configurator)
            where T : EndpointFactoryConfigurator
        {
            configurator.SupportMessageSerializer<BinaryMessageSerializer>();

            return configurator;
        }

        public static ServiceBusConfigurator SupportMessageSerializer<TSerializer>(
            this ServiceBusConfigurator configurator)
            where TSerializer : IMessageSerializer, new()
        {
            return SupportMessageSerializer(configurator, () => new TSerializer());
        }

        public static EndpointFactoryConfigurator SupportMessageSerializer<TSerializer>(
            this EndpointFactoryConfigurator configurator)
            where TSerializer : IMessageSerializer, new()
        {
            return SupportMessageSerializer(configurator, () => new TSerializer());
        }


        static T SetDefaultSerializer<T>(this T configurator, Func<IMessageSerializer> serializerFactory)
            where T : EndpointFactoryConfigurator
        {
            var serializerConfigurator = new DefaultSerializerEndpointFactoryConfigurator(serializerFactory);

            configurator.AddEndpointFactoryConfigurator(serializerConfigurator);

            return configurator;
        }

        static T SupportMessageSerializer<T>(this T configurator, Func<IMessageSerializer> serializerFactory)
            where T : EndpointFactoryConfigurator
        {
            var serializerConfigurator = new AddSerializerEndpointFactoryConfigurator(serializerFactory);

            configurator.AddEndpointFactoryConfigurator(serializerConfigurator);

            return configurator;
        }

        /// <summary>
        /// Sets the default message serializer for endpoints
        /// </summary>
        /// <typeparam name="TSerializer"></typeparam>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public static EndpointFactoryConfigurator SetDefaultSerializer<TSerializer>(
            this EndpointFactoryConfigurator configurator)
            where TSerializer : IMessageSerializer, new()
        {
            return SetDefaultSerializer(configurator, () => new TSerializer());
        }

        /// <summary>
        /// Sets the default message serializer for endpoints
        /// </summary>
        /// <typeparam name="TSerializer"></typeparam>
        /// <param name="configurator"></param>
        /// <returns></returns>
        public static ServiceBusConfigurator SetDefaultSerializer<TSerializer>(this ServiceBusConfigurator configurator)
            where TSerializer : IMessageSerializer, new()
        {
            return SetDefaultSerializer(configurator, () => new TSerializer());
        }

        /// <summary>
        /// Sets the default message serializer for endpoints
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="serializerType"></param>
        /// <returns></returns>
        public static T SetDefaultSerializer<T>(this T configurator,
            Type serializerType)
            where T : EndpointFactoryConfigurator
        {
            return SetDefaultSerializer(configurator, () => (IMessageSerializer)FastActivator.Create(serializerType));
        }

        /// <summary>
        /// Sets the default message serializer for endpoints
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public static T SetDefaultSerializer<T>(this T configurator,
            IMessageSerializer serializer)
            where T : EndpointFactoryConfigurator
        {
            return SetDefaultSerializer(configurator, () => serializer);
        }

        // -----------------------------------------------------------------------

        public static ServiceBusConfigurator SetSupportedMessageSerializers<T>(
            this ServiceBusConfigurator configurator)
            where T : ISupportedMessageSerializers, new()
        {
            return SetSupportedMessageSerializers(configurator, () => new T());
        }
        
        public static EndpointFactoryConfigurator SetSupportedMessageSerializers<T>(
            this EndpointFactoryConfigurator configurator)
            where T : ISupportedMessageSerializers, new()
        {
            return SetSupportedMessageSerializers(configurator, () => new T());
        }

        /// <summary>
        /// Sets the default message serializer for endpoints
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="supportedSerializer"></param>
        /// <returns></returns>
        public static T SetSupportedMessageSerializers<T>(this T configurator,
            ISupportedMessageSerializers supportedSerializer)
            where T : EndpointFactoryConfigurator
        {
            return SetSupportedMessageSerializers(configurator, () => supportedSerializer);
        }

        static T SetSupportedMessageSerializers<T>(this T configurator, Func<ISupportedMessageSerializers> supportedSerializers)
           where T : EndpointFactoryConfigurator
        {
            var serializerConfigurator = new SetSupportedMessageSerializersEndpointFactoryConfigurator(supportedSerializers);

            configurator.AddEndpointFactoryConfigurator(serializerConfigurator);

            return configurator;
        }
    }
}