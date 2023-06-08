namespace MassTransit.Configuration
{
    using MassTransit.Serialization;

    public static class ProtobufConfigurationExtensions
    {
        /// <summary>
        /// Registers the Protobuf serializer with the bus, using the default Protobuf message contract.
        /// </summary>
        public static void UseProtobufSerializer(this IBusFactoryConfigurator configurator)
        {
            var factory = new ProtobufSerializerFactory<object>();

            configurator.AddSerializer(factory);
        }

        /// <summary>
        /// Register the Protobuf deserializer for a specific message type on the receive endpoint.
        /// </summary>
        public static void UseProtobufDeserializer<TProtoMessage>(this IReceiveEndpointConfigurator configurator, bool isDefault = true)
            where TProtoMessage : class
        {
            var factory = new ProtobufSerializerFactory<TProtoMessage>();

            configurator.AddDeserializer(factory, isDefault);
        }
    }
}
