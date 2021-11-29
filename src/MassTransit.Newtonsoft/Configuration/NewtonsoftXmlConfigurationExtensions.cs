namespace MassTransit
{
    using Serialization;


    public static class NewtonsoftXmlConfigurationExtensions
    {
        /// <summary>
        /// Add the Newtonsoft XML serializer/deserializer to the bus
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseXmlSerializer(this IBusFactoryConfigurator configurator)
        {
            var factory = new NewtonsoftXmlSerializerFactory();

            configurator.AddSerializer(factory);
            configurator.AddDeserializer(factory, true);
        }

        /// <summary>
        /// Add the Newtonsoft XML serializer/deserializer to the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseXmlSerializer(this IReceiveEndpointConfigurator configurator)
        {
            var factory = new NewtonsoftXmlSerializerFactory();

            configurator.AddSerializer(factory);
            configurator.AddDeserializer(factory, true);
        }

        /// <summary>
        /// Add the Newtonsoft XML deserializer to the bus
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="isDefault"></param>
        public static void UseXmlDeserializer(this IBusFactoryConfigurator configurator, bool isDefault = false)
        {
            var factory = new NewtonsoftXmlSerializerFactory();

            configurator.AddDeserializer(factory, isDefault);
        }

        /// <summary>
        /// Add the Newtonsoft XML deserializer to the receive endpoint
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="isDefault"></param>
        public static void UseXmlDeserializer(this IReceiveEndpointConfigurator configurator, bool isDefault = false)
        {
            var factory = new NewtonsoftXmlSerializerFactory();

            configurator.AddDeserializer(factory, isDefault);
        }
    }
}
