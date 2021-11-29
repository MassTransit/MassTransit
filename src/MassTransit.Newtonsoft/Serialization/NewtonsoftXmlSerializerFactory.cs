namespace MassTransit.Serialization
{
    using System.Net.Mime;


    public class NewtonsoftXmlSerializerFactory :
        ISerializerFactory
    {
        public ContentType ContentType => NewtonsoftXmlMessageSerializer.XmlContentType;

        public IMessageSerializer CreateSerializer()
        {
            return new NewtonsoftXmlMessageSerializer();
        }

        public IMessageDeserializer CreateDeserializer()
        {
            return new NewtonsoftXmlMessageDeserializer(NewtonsoftXmlJsonMessageSerializer.Deserializer);
        }
    }
}
