namespace MassTransit
{
    using Configuration;


    public static class RawJsonSerializerConfigurationExtensions
    {
        /// <summary>
        /// Serialize and deserialize messages using the raw JSON message serializer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="options">Options for the raw serializer behavior</param>
        /// <param name="isDefault">If true, set the default content type to the content type of the deserializer</param>
        public static void UseRawJsonSerializer(this IBusFactoryConfigurator configurator, RawSerializerOptions options = RawSerializerOptions.Default,
            bool isDefault = false)
        {
            var factory = new SystemTextJsonRawMessageSerializerFactory(options);

            configurator.AddSerializer(factory);
            configurator.AddDeserializer(factory, isDefault);
        }

        /// <summary>
        /// Add support for RAW JSON message serialization and deserialization (does not change the default serializer)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="options">Options for the raw serializer behavior</param>
        public static void AddRawJsonSerializer(this IBusFactoryConfigurator configurator, RawSerializerOptions options =
            RawSerializerOptions.AddTransportHeaders | RawSerializerOptions.CopyHeaders)
        {
            var factory = new SystemTextJsonRawMessageSerializerFactory(options);

            configurator.AddSerializer(factory, false);
            configurator.AddDeserializer(factory);
        }

        /// <summary>
        /// Deserialize messages using the raw JSON message serializer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="options">Options for the raw serializer behavior</param>
        /// <param name="isDefault">If true, set the default content type to the content type of the deserializer</param>
        public static void UseRawJsonDeserializer(this IBusFactoryConfigurator configurator, RawSerializerOptions options = RawSerializerOptions.Default,
            bool isDefault = false)
        {
            var factory = new SystemTextJsonRawMessageSerializerFactory(options);

            configurator.AddDeserializer(factory, isDefault);
        }

        /// <summary>
        /// Serialize and deserialize messages using the raw JSON message serializer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="options">Options for the raw serializer behavior</param>
        /// <param name="isDefault">If true, set the default content type to the content type of the deserializer</param>
        public static void UseRawJsonSerializer(this IReceiveEndpointConfigurator configurator, RawSerializerOptions options = RawSerializerOptions.Default,
            bool isDefault = false)
        {
            var factory = new SystemTextJsonRawMessageSerializerFactory(options);

            configurator.AddSerializer(factory);
            configurator.AddDeserializer(factory, isDefault);
        }

        /// <summary>
        /// Deserialize messages using the raw JSON message serializer
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="options">Options for the raw serializer behavior</param>
        /// <param name="isDefault">If true, set the default content type to the content type of the deserializer</param>
        public static void UseRawJsonDeserializer(this IReceiveEndpointConfigurator configurator, RawSerializerOptions options = RawSerializerOptions.Default,
            bool isDefault = false)
        {
            var factory = new SystemTextJsonRawMessageSerializerFactory(options);

            configurator.AddDeserializer(factory, isDefault);
        }
    }
}
