namespace MassTransit
{
    using System;
    using Newtonsoft.Json;
    using Serialization;


    public static class NewtonsoftJsonConfigurationExtensions
    {
        /// <summary>
        /// Add the Newtonsoft JSON serializer/deserializer to the bus
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseNewtonsoftJsonSerializer(this IBusFactoryConfigurator configurator)
        {
            var factory = new NewtonsoftJsonSerializerFactory();

            configurator.AddSerializer(factory);
            configurator.AddDeserializer(factory, true);
        }

        /// <summary>
        /// Add the Newtonsoft JSON serializer/deserializer to the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseNewtonsoftJsonSerializer(this IReceiveEndpointConfigurator configurator)
        {
            var factory = new NewtonsoftJsonSerializerFactory();

            configurator.AddSerializer(factory);
            configurator.AddDeserializer(factory, true);
        }

        /// <summary>
        /// Add the Newtonsoft JSON deserializer to the bus
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="isDefault"></param>
        public static void UseNewtonsoftJsonDeserializer(this IBusFactoryConfigurator configurator, bool isDefault = false)
        {
            var factory = new NewtonsoftJsonSerializerFactory();

            configurator.AddDeserializer(factory, isDefault);
        }

        /// <summary>
        /// Add the Newtonsoft JSON deserializer to the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="isDefault"></param>
        public static void UseNewtonsoftJsonDeserializer(this IReceiveEndpointConfigurator configurator, bool isDefault = false)
        {
            var factory = new NewtonsoftJsonSerializerFactory();

            configurator.AddDeserializer(factory, isDefault);
        }

        /// <summary>
        /// Configure the Newtonsoft JSON serializer (does NOT add it to the bus as a message serializer)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <remarks>
        /// These settings are applied globally to <see cref="NewtonsoftJsonMessageSerializer.SerializerSettings" />.
        /// </remarks>
        public static void ConfigureNewtonsoftJsonSerializer(this IBusFactoryConfigurator configurator,
            Func<JsonSerializerSettings, JsonSerializerSettings> configure)
        {
            NewtonsoftJsonMessageSerializer.SerializerSettings = configure(NewtonsoftJsonMessageSerializer.SerializerSettings);
        }

        /// <summary>
        /// Configure the Newtonsoft JSON deserializer and add it to the bus as a message deserializer.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <remarks>
        /// These settings are applied globally to <see cref="NewtonsoftJsonMessageSerializer.SerializerSettings" />.
        /// </remarks>
        public static void ConfigureNewtonsoftJsonDeserializer(this IBusFactoryConfigurator configurator,
            Func<JsonSerializerSettings, JsonSerializerSettings> configure)
        {
            NewtonsoftJsonMessageSerializer.DeserializerSettings = configure(NewtonsoftJsonMessageSerializer.DeserializerSettings);
        }
    }
}
