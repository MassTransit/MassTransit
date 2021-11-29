namespace MassTransit
{
    using System;
    using Newtonsoft.Json;
    using Serialization;


    public static class NewtonsoftBsonConfigurationExtensions
    {
        /// <summary>
        /// Add the Newtonsoft BSON serializer/deserializer to the bus
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseBsonSerializer(this IBusFactoryConfigurator configurator)
        {
            var factory = new NewtonsoftBsonSerializerFactory();

            configurator.AddSerializer(factory);
            configurator.AddDeserializer(factory);
        }

        /// <summary>
        /// Add the Newtonsoft BSON deserializer to the bus
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="isDefault"></param>
        public static void UseBsonDeserializer(this IBusFactoryConfigurator configurator, bool isDefault = false)
        {
            var factory = new NewtonsoftBsonSerializerFactory();

            configurator.AddDeserializer(factory, isDefault);
        }

        /// <summary>
        /// Add the Newtonsoft BSON serializer/deserializer to the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseBsonSerializer(this IReceiveEndpointConfigurator configurator)
        {
            var factory = new NewtonsoftBsonSerializerFactory();

            configurator.AddSerializer(factory);
            configurator.AddDeserializer(factory);
        }

        /// <summary>
        /// Add the Newtonsoft BSON deserializer to the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="isDefault"></param>
        public static void UseBsonDeserializer(this IReceiveEndpointConfigurator configurator, bool isDefault = false)
        {
            var factory = new NewtonsoftBsonSerializerFactory();

            configurator.AddDeserializer(factory, isDefault);
        }

        /// <summary>
        /// Configure the Newtonsoft BSON serializer (does NOT add it to the bus as a message serializer)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <remarks>
        /// These settings are applied globally to <see cref="NewtonsoftJsonMessageSerializer.SerializerSettings" />.
        /// </remarks>
        public static void ConfigureBsonSerializer(this IBusFactoryConfigurator configurator,
            Func<JsonSerializerSettings, JsonSerializerSettings> configure = null)
        {
            if (configure != null)
                BsonMessageSerializer.SerializerSettings = configure(BsonMessageSerializer.SerializerSettings);
        }

        /// <summary>
        /// Configure the Newtonsoft BSON deserializer and add it to the bus as a message deserializer.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <remarks>
        /// These settings are applied globally to <see cref="NewtonsoftJsonMessageSerializer.SerializerSettings" />.
        /// </remarks>
        public static void ConfigureBsonDeserializer(this IBusFactoryConfigurator configurator,
            Func<JsonSerializerSettings, JsonSerializerSettings> configure = null)
        {
            if (configure != null)
                BsonMessageSerializer.DeserializerSettings = configure(BsonMessageSerializer.DeserializerSettings);
        }
    }
}
