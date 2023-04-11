namespace MassTransit.Serialization
{
    using System.Net.Mime;


    public class EncryptedFallbackSerializerFactoryV2 :
        ISerializerFactory
    {
        readonly ICryptoStreamProviderV2 _cryptoStream;
        readonly ICryptoStreamProviderV2 _fallbackCryptoStream;

        public EncryptedFallbackSerializerFactoryV2(ICryptoStreamProviderV2 cryptoStream, ICryptoStreamProviderV2 fallbackCryptoStream)
        {
            _cryptoStream = cryptoStream;
            _fallbackCryptoStream = fallbackCryptoStream;
        }

        public ContentType ContentType => EncryptedMessageSerializerV2.EncryptedContentType;

        public IMessageSerializer CreateSerializer()
        {
            return new EncryptedMessageSerializerV2(_cryptoStream);
        }

        public IMessageDeserializer CreateDeserializer()
        {
            return new EncryptedFallbackMessageDeserializerV2(_cryptoStream, _fallbackCryptoStream);
        }
    }
}
