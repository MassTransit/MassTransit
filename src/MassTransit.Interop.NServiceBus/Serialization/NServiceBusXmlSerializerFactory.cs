namespace MassTransit.Serialization
{
    using System.Net.Mime;


    public class NServiceBusXmlSerializerFactory :
        ISerializerFactory
    {
        public ContentType ContentType => NServiceBusXmlMessageSerializer.XmlContentType;

        public IMessageSerializer CreateSerializer()
        {
            return new NServiceBusXmlMessageSerializer();
        }

        public IMessageDeserializer CreateDeserializer()
        {
            return new NServiceBusXmlMessageDeserializer(NewtonsoftJsonMessageSerializer.Deserializer);
        }
    }
}
