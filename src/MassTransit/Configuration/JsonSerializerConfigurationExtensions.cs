namespace MassTransit
{
    using System;
    using System.Text.Json;
    using Configuration;
    using Serialization;


    public static class JsonSerializerConfigurationExtensions
    {
        /// <summary>
        /// Serialize messages using the raw JSON message serializer
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseJsonSerializer(this IBusFactoryConfigurator configurator)
        {
            var factory = new SystemTextJsonMessageSerializerFactory();

            configurator.AddSerializer(factory);
            configurator.AddDeserializer(factory);
        }

        /// <summary>
        /// Serialize messages using the raw JSON message serializer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="isDefault">If true, set the default content type to the content type of the deserializer</param>
        public static void UseJsonDeserializer(this IBusFactoryConfigurator configurator, bool isDefault = false)
        {
            var factory = new SystemTextJsonMessageSerializerFactory();

            configurator.AddDeserializer(factory, isDefault);
        }

        /// <summary>
        /// Serialize messages using the raw JSON message serializer
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseJsonSerializer(this IReceiveEndpointConfigurator configurator)
        {
            var factory = new SystemTextJsonMessageSerializerFactory();

            configurator.AddSerializer(factory);
            configurator.AddDeserializer(factory);
        }

        /// <summary>
        /// Serialize messages using the raw JSON message serializer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="isDefault">If true, set the default content type to the content type of the deserializer</param>
        public static void UseJsonDeserializer(this IReceiveEndpointConfigurator configurator, bool isDefault = false)
        {
            var factory = new SystemTextJsonMessageSerializerFactory();

            configurator.AddDeserializer(factory, isDefault);
        }

        /// <summary>
        /// Configure the global shared options for the default System.Text.Json serializer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void ConfigureJsonSerializerOptions(this IBusFactoryConfigurator configurator,
            Func<JsonSerializerOptions, JsonSerializerOptions> configure = null)
        {
            if (configure != null)
                SystemTextJsonMessageSerializer.Options = configure(new JsonSerializerOptions(SystemTextJsonMessageSerializer.Options));
        }
    }
}
