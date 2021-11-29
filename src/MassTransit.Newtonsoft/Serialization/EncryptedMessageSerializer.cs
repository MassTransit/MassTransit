#nullable enable
namespace MassTransit.Serialization
{
    using System.Net.Mime;


    public class EncryptedMessageSerializer :
        IMessageSerializer
    {
        public const string ContentTypeHeaderValue = "application/vnd.masstransit+aes";
        public const string EncryptionKeyHeader = "EncryptionKeyId";
        public static readonly ContentType EncryptedContentType = new ContentType(ContentTypeHeaderValue);
        readonly BsonMessageSerializer _bsonMessageSerializer;
        readonly ICryptoStreamProvider _streamProvider;

        public EncryptedMessageSerializer(ICryptoStreamProvider streamProvider)
        {
            _streamProvider = streamProvider;
            _bsonMessageSerializer = new BsonMessageSerializer();
        }

        public ContentType ContentType => EncryptedContentType;

        public MessageBody GetMessageBody<T>(SendContext<T> context)
            where T : class
        {
            var clearMessageBody = _bsonMessageSerializer.GetMessageBody(context);

            return new EncryptMessageBody<T>(_streamProvider, context, clearMessageBody);
        }
    }
}
