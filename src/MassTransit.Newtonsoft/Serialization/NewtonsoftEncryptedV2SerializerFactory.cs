namespace MassTransit.Serialization
{
    using System.Net.Mime;


    public class NewtonsoftEncryptedV2SerializerFactory :
        ISerializerFactory
    {
        readonly ICryptoStreamProviderV2 _provider;

        public NewtonsoftEncryptedV2SerializerFactory(ICryptoStreamProviderV2 provider)
        {
            _provider = provider;
        }

        public ContentType ContentType => EncryptedMessageSerializerV2.EncryptedContentType;

        public IMessageSerializer CreateSerializer()
        {
            return new EncryptedMessageSerializerV2(_provider);
        }

        public IMessageDeserializer CreateDeserializer()
        {
            return new EncryptedMessageDeserializerV2(BsonMessageSerializer.Deserializer, _provider);
        }
    }
}
