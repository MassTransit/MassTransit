namespace MassTransit.Serialization
{
    using System.Net.Mime;


    public class NServiceBusJsonSerializerFactory :
        ISerializerFactory
    {
        public ContentType ContentType => NServiceBusJsonMessageSerializer.JsonContentType;

        public IMessageSerializer CreateSerializer()
        {
            return new NServiceBusJsonMessageSerializer();
        }

        public IMessageDeserializer CreateDeserializer()
        {
            return new NServiceBusJsonMessageDeserializer(NewtonsoftJsonMessageSerializer.Deserializer);
        }
    }
}
