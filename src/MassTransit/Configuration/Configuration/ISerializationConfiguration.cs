namespace MassTransit.Configuration
{
    using System.Net.Mime;


    public interface ISerializationConfiguration :
        ISpecification
    {
        /// <summary>
        /// When deserializing a message, if no ContentType is present on the receive context, use this as the default
        /// </summary>
        ContentType DefaultContentType { set; }

        /// <summary>
        /// When serializing a message, the content type of the serializer to use
        /// </summary>
        ContentType SerializerContentType { set; }

        void AddSerializer(ISerializerFactory factory, bool isSerializer = true);

        void AddDeserializer(ISerializerFactory factory, bool isDefault = false);

        /// <summary>
        /// Clear the configuration, removing all deserializers, serializers, and breaking the
        /// linkage to the bus serialization configuration.
        /// </summary>
        void Clear();

        ISerializationConfiguration CreateSerializationConfiguration();

        /// <summary>
        /// Compiles the configured serializers into a collection for use by the receive endpoint
        /// </summary>
        /// <returns></returns>
        ISerialization CreateSerializerCollection();
    }
}
