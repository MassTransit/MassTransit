namespace MassTransit.Serialization
{
    using System.Net.Mime;


    /// <summary>
    /// Copies the body of the receive context to the send context unmodified
    /// </summary>
    public class CopyBodySerializer :
        IMessageSerializer
    {
        readonly MessageBody _body;

        public CopyBodySerializer(ContentType contentType, MessageBody body)
        {
            _body = body;

            ContentType = contentType;
        }

        public ContentType ContentType { get; }

        public MessageBody GetMessageBody<T>(SendContext<T> context)
            where T : class
        {
            return _body;
        }
    }
}
