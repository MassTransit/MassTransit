namespace MassTransit.Serialization
{
    using System.Net.Mime;
    using MassTransit;


    public class DualNewtonsoftDecryptionV2SerializerFactory : ISerializerFactory
    {
        readonly ICryptoStreamProviderV2 _primaryCryptoStream;
        readonly ICryptoStreamProviderV2 _secondaryCryptoStream;

        public DualNewtonsoftDecryptionV2SerializerFactory(ICryptoStreamProviderV2 primaryCryptoStream, ICryptoStreamProviderV2 secondaryCryptoStream)
        {
            _primaryCryptoStream = primaryCryptoStream;
            _secondaryCryptoStream = secondaryCryptoStream;
        }

        public ContentType ContentType => EncryptedMessageSerializerV2.EncryptedContentType;

        public IMessageSerializer CreateSerializer()
        {
            return new EncryptedMessageSerializerV2(_primaryCryptoStream);
        }

        public IMessageDeserializer CreateDeserializer()
        {
            return new DualDecryptionMessageDeserializerV2(_primaryCryptoStream, _secondaryCryptoStream);
        }
    }
}
