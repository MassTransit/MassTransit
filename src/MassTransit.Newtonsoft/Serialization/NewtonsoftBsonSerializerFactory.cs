namespace MassTransit.Serialization
{
    using System.Net.Mime;


    public class NewtonsoftBsonSerializerFactory :
        ISerializerFactory
    {
        public ContentType ContentType => BsonMessageSerializer.BsonContentType;

        public IMessageSerializer CreateSerializer()
        {
            return new BsonMessageSerializer();
        }

        public IMessageDeserializer CreateDeserializer()
        {
            return new NewtonsoftBsonMessageDeserializer(BsonMessageSerializer.Deserializer);
        }
    }
}
