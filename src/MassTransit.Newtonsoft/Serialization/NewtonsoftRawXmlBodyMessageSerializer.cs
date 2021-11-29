#nullable enable
namespace MassTransit.Serialization
{
    using System.Net.Mime;
    using Newtonsoft.Json.Linq;


    /// <summary>
    /// Used to serialize an existing deserialized message when a message is forwarded, scheduled, etc.
    /// </summary>
    public class NewtonsoftRawXmlBodyMessageSerializer :
        IMessageSerializer
    {
        readonly string[]? _messageTypes;
        readonly RawSerializerOptions _options;
        JToken _message;

        public NewtonsoftRawXmlBodyMessageSerializer(JToken message, ContentType contentType, RawSerializerOptions options,
            string[]? messageTypes = null)
        {
            _message = message;
            _options = options;
            _messageTypes = messageTypes;

            ContentType = contentType;
        }

        public ContentType ContentType { get; }

        public MessageBody GetMessageBody<T>(SendContext<T> context)
            where T : class
        {
            if (_messageTypes != null && _options.HasFlag(RawSerializerOptions.AddTransportHeaders))
                context.Headers.Set(MessageHeaders.MessageType, string.Join(";", _messageTypes));

            return new NewtonsoftRawXmlMessageBody<T>(context, _message);
        }

        public void Overlay(object message)
        {
            JToken overlayToken = JObject.FromObject(message, NewtonsoftJsonMessageSerializer.Serializer);

            _message = _message.Merge(overlayToken);
        }
    }
}
