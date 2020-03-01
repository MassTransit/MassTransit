namespace MassTransit.Serialization
{
    using System.IO;
    using System.Net.Mime;
    using Metadata;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;


    public class EncryptedMessageSerializerV2 :
        IMessageSerializer
    {
        public const string ContentTypeHeaderValue = "application/vnd.masstransit.v2+aes";
        public static readonly ContentType EncryptedContentType = new ContentType(ContentTypeHeaderValue);
        readonly JsonSerializer _serializer;
        readonly ICryptoStreamProviderV2 _streamProvider;

        public EncryptedMessageSerializerV2(ICryptoStreamProviderV2 streamProvider)
        {
            _streamProvider = streamProvider;
            _serializer = BsonMessageSerializer.Serializer;
        }

        ContentType IMessageSerializer.ContentType => EncryptedContentType;

        void IMessageSerializer.Serialize<T>(Stream stream, SendContext<T> context)
        {
            context.ContentType = EncryptedContentType;

            var envelope = new JsonMessageEnvelope(context, context.Message, TypeMetadataCache<T>.MessageTypeNames);

            using var cryptoStream = _streamProvider.GetEncryptStream(stream, context);
            using var jsonWriter = new BsonDataWriter(cryptoStream);

            _serializer.Serialize(jsonWriter, envelope, typeof(MessageEnvelope));

            jsonWriter.Flush();
        }
    }
}
