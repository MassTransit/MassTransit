#nullable enable
namespace MassTransit.Serialization
{
    using System.Net.Mime;
    using Newtonsoft.Json.Linq;


    /// <summary>
    /// Used to serialize an existing deserialized message when a message is forwarded, scheduled, etc.
    /// </summary>
    public class NewtonsoftBsonBodyMessageSerializer :
        IMessageSerializer
    {
        readonly JsonMessageEnvelope _envelope;

        public NewtonsoftBsonBodyMessageSerializer(MessageEnvelope envelope, ContentType contentType)
        {
            _envelope = new JsonMessageEnvelope(envelope);
            ContentType = contentType;
        }

        public ContentType ContentType { get; }

        public MessageBody GetMessageBody<T>(SendContext<T> context)
            where T : class
        {
            _envelope.Update(context);

            return new NewtonsoftBsonMessageBody<T>(context, _envelope);
        }

        public void Overlay(object message)
        {
            if (_envelope.Message is JToken messageToken)
            {
                JToken overlayToken = JObject.FromObject(message, NewtonsoftJsonMessageSerializer.Serializer);

                _envelope.Message = messageToken.Merge(overlayToken);
            }
            else
                _envelope.Message = message;
        }
    }
}
