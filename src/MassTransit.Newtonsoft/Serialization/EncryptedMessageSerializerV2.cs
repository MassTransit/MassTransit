namespace MassTransit.Serialization
{
    using System.Net.Mime;


    public class EncryptedMessageSerializerV2 :
        IMessageSerializer
    {
        public const string ContentTypeHeaderValue = "application/vnd.masstransit.v2+aes";
        public static readonly ContentType EncryptedContentType = new ContentType(ContentTypeHeaderValue);
        readonly ICryptoStreamProviderV2 _streamProvider;

        public EncryptedMessageSerializerV2(ICryptoStreamProviderV2 streamProvider)
        {
            _streamProvider = streamProvider;
        }

        public ContentType ContentType => EncryptedContentType;

        public MessageBody GetMessageBody<T>(SendContext<T> context)
            where T : class
        {
            var clearMessageBody = BsonMessageSerializer.Instance.GetMessageBody(context);

            return new EncryptMessageBodyV2<T>(_streamProvider, context, clearMessageBody);
        }
    }
}
