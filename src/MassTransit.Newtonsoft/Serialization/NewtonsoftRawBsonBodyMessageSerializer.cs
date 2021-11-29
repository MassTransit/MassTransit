#nullable enable
namespace MassTransit.Serialization
{
    using System.Net.Mime;
    using Newtonsoft.Json.Linq;


    /// <summary>
    /// Used to serialize an existing deserialized message when a message is forwarded, scheduled, etc.
    /// </summary>
    public class NewtonsoftRawBsonBodyMessageSerializer :
        IMessageSerializer
    {
        JToken _message;
        readonly string[]? _messageTypes;

        public NewtonsoftRawBsonBodyMessageSerializer(JToken message, ContentType contentType, string[]? messageTypes = null)
        {
            _message = message;
            _messageTypes = messageTypes;

            ContentType = contentType;
        }

        public ContentType ContentType { get; }

        public MessageBody GetMessageBody<T>(SendContext<T> context)
            where T : class
        {
            if (_messageTypes != null)
                context.Headers.Set(MessageHeaders.MessageType, string.Join(";", _messageTypes));

            return new NewtonsoftRawBsonMessageBody<T>(context, _message);
        }

        public void Overlay(object message)
        {
            JToken overlayToken = JObject.FromObject(message, NewtonsoftJsonMessageSerializer.Serializer);

            _message = _message.Merge(overlayToken);
        }
    }
}
