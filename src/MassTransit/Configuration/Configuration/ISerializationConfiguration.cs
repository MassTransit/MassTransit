namespace MassTransit.Configuration
{
    using System.Net.Mime;
    using GreenPipes;


    public interface ISerializationConfiguration :
        ISpecification
    {
        IMessageSerializer Serializer { get; }
        IMessageDeserializer Deserializer { get; }

        void AddDeserializer(ContentType contentType, DeserializerFactory deserializerFactory);
        void SetSerializer(SerializerFactory serializerFactory);
        void ClearDeserializers();

        /// <summary>
        /// The default content type is used if no transport content-type is available
        /// </summary>
        ContentType DefaultContentType { set; }

        ISerializationConfiguration CreateSerializationConfiguration();
    }
}
