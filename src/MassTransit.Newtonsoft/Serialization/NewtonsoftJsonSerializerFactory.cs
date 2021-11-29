namespace MassTransit.Serialization
{
    using System.Net.Mime;


    public class NewtonsoftJsonSerializerFactory :
        ISerializerFactory
    {
        public ContentType ContentType => NewtonsoftJsonMessageSerializer.JsonContentType;

        public IMessageSerializer CreateSerializer()
        {
            return new NewtonsoftJsonMessageSerializer();
        }

        public IMessageDeserializer CreateDeserializer()
        {
            return new NewtonsoftJsonMessageDeserializer(NewtonsoftJsonMessageSerializer.Deserializer);
        }
    }
}
