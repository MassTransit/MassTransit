namespace MassTransit
{
    using Serialization;


    public static class NServiceBusSerializerConfigurationExtensions
    {
        /// <summary>
        /// Use NServiceBus Json Serialization (Newtonsoft) for sent/published messages.
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseNServiceBusJsonSerializer(this IBusFactoryConfigurator configurator)
        {
            var factory = new NServiceBusJsonSerializerFactory();

            configurator.AddSerializer(factory, true);
            configurator.AddDeserializer(factory, true);
        }

        /// <summary>
        /// Support NServiceBus JSON deserialization
        /// </summary>
        /// <param name="configurator"></param>
        public static void SupportNServiceBusJsonDeserializer(this IBusFactoryConfigurator configurator)
        {
            var factory = new NServiceBusJsonSerializerFactory();

            configurator.AddDeserializer(factory);
        }

        /// <summary>
        /// Use NServiceBus XML Serialization for sent/published messages.
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseNServiceBusXmlSerializer(this IBusFactoryConfigurator configurator)
        {
            var factory = new NServiceBusXmlSerializerFactory();

            configurator.AddSerializer(factory, true);
            configurator.AddDeserializer(factory, true);
        }

        /// <summary>
        /// Support NServiceBus XML deserialization
        /// </summary>
        /// <param name="configurator"></param>
        public static void SupportNServiceBusXmlDeserializer(this IBusFactoryConfigurator configurator)
        {
            var factory = new NServiceBusXmlSerializerFactory();

            configurator.AddDeserializer(factory);
        }
    }
}
