namespace MassTransit
{
    using Serialization;


    public static class NewtonsoftRawXmlConfigurationExtensions
    {
        /// <summary>
        /// Add the Newtonsoft raw XML serializer/deserializer to the bus
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="options">Options for the raw serializer behavior</param>
        public static void UseRawXmlSerializer(this IBusFactoryConfigurator configurator, RawSerializerOptions options = RawSerializerOptions.Default)
        {
            var factory = new NewtonsoftRawXmlSerializerFactory(options);

            configurator.AddSerializer(factory);
            configurator.AddDeserializer(factory);
        }

        /// <summary>
        /// Add the Newtonsoft raw XML serializer/deserializer to the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="options">Options for the raw serializer behavior</param>
        public static void UseRawXmlSerializer(this IReceiveEndpointConfigurator configurator, RawSerializerOptions options = RawSerializerOptions.Default)
        {
            var factory = new NewtonsoftRawXmlSerializerFactory(options);

            configurator.AddSerializer(factory);
            configurator.AddDeserializer(factory);
        }

        /// <summary>
        /// Add the Newtonsoft raw XML deserializer to the bus
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="options">Options for the raw serializer behavior</param>
        public static void UseRawXmlDeserializer(this IBusFactoryConfigurator configurator, RawSerializerOptions options = RawSerializerOptions.Default)
        {
            var factory = new NewtonsoftRawXmlSerializerFactory(options);

            configurator.AddDeserializer(factory);
        }

        /// <summary>
        /// Add the Newtonsoft raw XML deserializer to the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="options">Options for the raw serializer behavior</param>
        public static void UseRawXmlDeserializer(this IReceiveEndpointConfigurator configurator, RawSerializerOptions options = RawSerializerOptions.Default)
        {
            var factory = new NewtonsoftRawXmlSerializerFactory(options);

            configurator.AddDeserializer(factory);
        }
    }
}
