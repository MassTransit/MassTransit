namespace MassTransit
{
    using Configuration;
    using Serialization;


    public static class RawJsonSerializerConfigurationExtensions
    {
        /// <summary>
        /// Serialize messages using the raw JSON message serializer
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
        /// Serialize messages using the raw JSON message serializer
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
        /// Serialize messages using the raw JSON message serializer
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
        /// Serialize messages using the raw JSON message serializer
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
