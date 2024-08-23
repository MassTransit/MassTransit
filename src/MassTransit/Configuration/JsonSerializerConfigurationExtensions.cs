#nullable enable
namespace MassTransit
{
    using System;
    using System.Linq;
    using System.Text.Json;
    using Configuration;
    using Serialization;
    using Serialization.JsonConverters;


    public static class JsonSerializerConfigurationExtensions
    {
        /// <summary>
        /// Serialize and deserialize messages using the raw JSON message serializer
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseJsonSerializer(this IBusFactoryConfigurator configurator)
        {
            var factory = new SystemTextJsonMessageSerializerFactory();

            configurator.AddSerializer(factory);
            configurator.AddDeserializer(factory);
        }

        /// <summary>
        /// Deserialize messages using the raw JSON message serializer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="isDefault">If true, set the default content type to the content type of the deserializer</param>
        public static void UseJsonDeserializer(this IBusFactoryConfigurator configurator, bool isDefault = false)
        {
            var factory = new SystemTextJsonMessageSerializerFactory();

            configurator.AddDeserializer(factory, isDefault);
        }

        /// <summary>
        /// Serialize and deserialize messages using the raw JSON message serializer
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseJsonSerializer(this IReceiveEndpointConfigurator configurator)
        {
            var factory = new SystemTextJsonMessageSerializerFactory();

            configurator.AddSerializer(factory);
            configurator.AddDeserializer(factory);
        }

        /// <summary>
        /// Deserialize messages using the raw JSON message serializer
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
            Func<JsonSerializerOptions, JsonSerializerOptions>? configure = null)
        {
            if (configure != null)
                SystemTextJsonMessageSerializer.Options = configure(new JsonSerializerOptions(SystemTextJsonMessageSerializer.Options));
        }

        /// <summary>
        /// Specify custom <see cref="JsonSerializerOptions"/> for a message type, removing any previous configured options for the same message type.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T"></typeparam>
        public static void SetMessageSerializerOptions<T>(this JsonSerializerOptions options,
            Func<JsonSerializerOptions, JsonSerializerOptions>? configure = null)
            where T : class
        {
            var existingConverter = options.Converters.FirstOrDefault(x => x is CustomMessageTypeJsonConverter<T>);
            if(existingConverter != null)
                options.Converters.Remove(existingConverter);

            var messageSerializerOptions = new JsonSerializerOptions();
            configure?.Invoke(messageSerializerOptions);

            options.Converters.Insert(0, new CustomMessageTypeJsonConverter<T>(messageSerializerOptions));
        }
    }
}
