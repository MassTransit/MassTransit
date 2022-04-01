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
        [Obsolete("Use new overload that accepts a configure delegate as it does not depend on global static state.")]
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
        [Obsolete("Use new overload that accepts a configure delegate as it does not depend on global static state.")]
        public static void UseJsonDeserializer(this IBusFactoryConfigurator configurator, bool isDefault = false)
        {
            var factory = new SystemTextJsonMessageSerializerFactory();

            configurator.AddDeserializer(factory, isDefault);
        }

        /// <summary>
        /// Serialize messages using the raw JSON message serializer
        /// </summary>
        /// <param name="configurator"></param>
        [Obsolete("Use new overload that accepts a configure delegate as it does not depend on global static state.")]
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
        [Obsolete("Use new overload that accepts a configure delegate as it does not depend on global static state.")]
        public static void UseJsonDeserializer(this IReceiveEndpointConfigurator configurator, bool isDefault = false)
        {
            var factory = new SystemTextJsonMessageSerializerFactory();

            configurator.AddDeserializer(factory, isDefault);
        }

        /// <summary>
        /// Configure the options for the default System.Text.Json serializer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        [Obsolete("Use an overload of UseJsonSerializer instead.")]
        public static void ConfigureJsonSerializerOptions(this IBusFactoryConfigurator configurator,
            Func<JsonSerializerOptions, JsonSerializerOptions> configure = null)
        {
            if (configure != null)
                SystemTextJsonMessageSerializer.Options = configure(SystemTextJsonMessageSerializer.Options);
        }

        /// <summary>
        /// Serialize messages using the raw JSON message serializer
        /// </summary>
        /// <param name="configurator">The <see cref="IBusFactoryConfigurator"/>.</param>
        /// <param name="configure">Optional configuration delegate to configure the json serializer.</param>
        public static void UseJsonSerializer(
            this IBusFactoryConfigurator configurator,
            Action<JsonSerializerOptions> configure)
            => UseJsonSerializer(configurator, SystemTextJsonMessageSerializer.DefaultOptions, configure);

        /// <summary>
        /// Deserialize messages using the raw JSON message serializer
        /// </summary>
        /// <param name="configurator">The <see cref="IBusFactoryConfigurator"/>.</param>
        /// <param name="configure">Optional configuration delegate to configure the json serializer.</param>
        /// <param name="isDefault">Whether or not this is the default deserializer.</param>
        public static void UseJsonDeserializer(
            this IBusFactoryConfigurator configurator,
            Action<JsonSerializerOptions> configure,
            bool isDefault = false)
            => UseJsonSerializer(configurator, SystemTextJsonMessageSerializer.DefaultOptions, configure, addSerializer: false, defaultDeserializer: isDefault);

        /// <summary>
        /// Serialize messages using the raw JSON message serializer
        /// </summary>
        /// <param name="configurator">The <see cref="IReceiveEndpointConfigurator"/>.</param>
        /// <param name="configure">Optional configuration delegate to configure the json serializer.</param>
        public static void UseJsonSerializer(
            this IReceiveEndpointConfigurator configurator,
            Action<JsonSerializerOptions> configure)
            => UseJsonSerializer(configurator, SystemTextJsonMessageSerializer.DefaultOptions, configure);

        /// <summary>
        /// Deserialize messages using the raw JSON message serializer
        /// </summary>
        /// <param name="configurator">The <see cref="IReceiveEndpointConfigurator"/>.</param>
        /// <param name="configure">Optional configuration delegate to configure the json serializer.</param>
        /// <param name="isDefault">Whether or not this is the default deserializer.</param>
        public static void UseJsonDeserializer(
            this IReceiveEndpointConfigurator configurator,
            Action<JsonSerializerOptions> configure,
            bool isDefault = false)
            => UseJsonSerializer(configurator, SystemTextJsonMessageSerializer.DefaultOptions, configure, addSerializer: false, defaultDeserializer: isDefault);

        static void UseJsonSerializer(
            this IBusFactoryConfigurator configurator,
            JsonSerializerOptions defaultOptions,
            Action<JsonSerializerOptions> configure,
            bool addSerializer = true,
            bool addDeserializer = true,
            bool defaultDeserializer = false)
        {
            var options = defaultOptions;
            if (configure != null)
            {
                options = new JsonSerializerOptions(options);
                configure(new JsonSerializerOptions(options));
            }

            var factory = new SystemTextJsonMessageSerializerFactory(options);

            if (addSerializer)
            {
                configurator.AddSerializer(factory);
            }

            if (addDeserializer)
            {
                configurator.AddDeserializer(factory, defaultDeserializer);
            }
        }

        static void UseJsonSerializer(
            this IReceiveEndpointConfigurator configurator,
            JsonSerializerOptions defaultOptions,
            Action<JsonSerializerOptions> configure,
            bool addSerializer = true,
            bool addDeserializer = true,
            bool defaultDeserializer = false)
        {
            var options = defaultOptions;
            if (configure != null)
            {
                options = new JsonSerializerOptions(options);
                configure(new JsonSerializerOptions(options));
            }

            var factory = new SystemTextJsonMessageSerializerFactory(options);

            if (addSerializer)
            {
                configurator.AddSerializer(factory);
            }

            if (addDeserializer)
            {
                configurator.AddDeserializer(factory, defaultDeserializer);
            }
        }
    }
}
