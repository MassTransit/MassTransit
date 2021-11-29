namespace MassTransit
{
    using System;


    public static class LegacySerializationExtensions
    {
        [Obsolete("Deprecated, use ClearSerialization instead")]
        public static void ClearMessageDeserializers(this IBusFactoryConfigurator configurator)
        {
            configurator.ClearSerialization();
        }

        [Obsolete("Deprecated, use ClearSerialization instead")]
        public static void ClearMessageDeserializers(this IReceiveEndpointConfigurator configurator)
        {
            configurator.ClearSerialization();
        }
    }
}
