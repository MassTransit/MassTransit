namespace MassTransit
{
    using Serialization;


    public static class NewtonsoftRawJsonConfigurationExtensions
    {
        /// <summary>
        /// Add the Newtonsoft raw JSON serializer/deserializer to the bus
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="options">Options for the raw serializer behavior</param>
        public static void UseNewtonsoftRawJsonSerializer(this IBusFactoryConfigurator configurator,
            RawSerializerOptions options = RawSerializerOptions.Default)
        {
            var factory = new NewtonsoftRawJsonSerializerFactory(options);

            configurator.AddSerializer(factory);
            configurator.AddDeserializer(factory);
        }

        /// <summary>
        /// Add support for RAW JSON message serialization and deserialization using Newtonsoft (does not change the default serializer)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="options">Options for the raw serializer behavior</param>
        public static void AddNewtonsoftRawJsonSerializer(this IBusFactoryConfigurator configurator, RawSerializerOptions options =
            RawSerializerOptions.AddTransportHeaders | RawSerializerOptions.CopyHeaders)
        {
            var factory = new NewtonsoftRawJsonSerializerFactory(options);

            configurator.AddSerializer(factory, false);
            configurator.AddDeserializer(factory);
        }

        /// <summary>
        /// Add the Newtonsoft raw JSON deserializer to the bus
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="options">Options for the raw serializer behavior</param>
        public static void UseNewtonsoftRawJsonDeserializer(this IBusFactoryConfigurator configurator,
            RawSerializerOptions options = RawSerializerOptions.Default)
        {
            var factory = new NewtonsoftRawJsonSerializerFactory(options);

            configurator.AddDeserializer(factory);
        }

        /// <summary>
        /// Add the Newtonsoft raw JSON serializer/deserializer to the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="options">Options for the raw serializer behavior</param>
        public static void UseNewtonsoftRawJsonSerializer(this IReceiveEndpointConfigurator configurator,
            RawSerializerOptions options = RawSerializerOptions.Default)
        {
            var factory = new NewtonsoftRawJsonSerializerFactory(options);

            configurator.AddSerializer(factory);
            configurator.AddDeserializer(factory);
        }

        /// <summary>
        /// Add the Newtonsoft raw JSON deserializer to the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="options">Options for the raw serializer behavior</param>
        public static void UseNewtonsoftRawJsonDeserializer(this IReceiveEndpointConfigurator configurator,
            RawSerializerOptions options = RawSerializerOptions.Default)
        {
            var factory = new NewtonsoftRawJsonSerializerFactory(options);

            configurator.AddDeserializer(factory);
        }
    }
}
