namespace MassTransit.Serialization
{
    using System.IO;
    using System.Net.Mime;
    using Metadata;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;


    public class EncryptedMessageSerializer :
        IMessageSerializer
    {
        public const string ContentTypeHeaderValue = "application/vnd.masstransit+aes";
        public const string EncryptionKeyHeader = "EncryptionKeyId";
        public static readonly ContentType EncryptedContentType = new ContentType(ContentTypeHeaderValue);
        readonly JsonSerializer _serializer;
        readonly ICryptoStreamProvider _streamProvider;

        public EncryptedMessageSerializer(ICryptoStreamProvider streamProvider)
        {
            _streamProvider = streamProvider;
            _serializer = BsonMessageSerializer.Serializer;
        }

        ContentType IMessageSerializer.ContentType => EncryptedContentType;

        void IMessageSerializer.Serialize<T>(Stream stream, SendContext<T> context)
        {
            context.ContentType = EncryptedContentType;

            var envelope = new JsonMessageEnvelope(context, context.Message, TypeMetadataCache<T>.MessageTypeNames);

            using Stream cryptoStream = _streamProvider.GetEncryptStream(stream, context);
            using var jsonWriter = new BsonDataWriter(cryptoStream);

            _serializer.Serialize(jsonWriter, envelope, typeof(MessageEnvelope));

            jsonWriter.Flush();
        }
    }
}
