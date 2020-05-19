namespace MassTransit
{
    using Interop.NServiceBus.Serialization;
    using Serialization;


    public static class NServiceBusSerializerConfigurationExtensions
    {
        /// <summary>
        /// Use NServiceBus Json Serialization (Newtonsoft) for sent/published messages.
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseNServiceBusJsonSerializer(this IBusFactoryConfigurator configurator)
        {
            configurator.SetMessageSerializer(() => new NServiceBusJsonMessageSerializer());

            SupportNServiceBusJsonDeserializer(configurator);
        }

        /// <summary>
        /// Support NServiceBus JSON deserialization
        /// </summary>
        /// <param name="configurator"></param>
        public static void SupportNServiceBusJsonDeserializer(this IBusFactoryConfigurator configurator)
        {
            configurator.AddMessageDeserializer(NServiceBusJsonMessageSerializer.JsonContentType,
                () => new NServiceBusJsonMessageDeserializer(JsonMessageSerializer.Deserializer));
        }

        /// <summary>
        /// Use NServiceBus XML Serialization for sent/published messages.
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseNServiceBusXmlSerializer(this IBusFactoryConfigurator configurator)
        {
            configurator.SetMessageSerializer(() => new NServiceBusXmlMessageSerializer());

            SupportNServiceBusXmlDeserializer(configurator);
        }

        /// <summary>
        /// Support NServiceBus XML deserialization
        /// </summary>
        /// <param name="configurator"></param>
        public static void SupportNServiceBusXmlDeserializer(this IBusFactoryConfigurator configurator)
        {
            configurator.AddMessageDeserializer(NServiceBusXmlMessageDeserializer.XmlContentType,
                () => new NServiceBusXmlMessageDeserializer(JsonMessageSerializer.Deserializer));
        }
    }
}
