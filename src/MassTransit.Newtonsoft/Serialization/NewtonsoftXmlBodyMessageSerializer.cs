#nullable enable
namespace MassTransit.Serialization
{
    using System.Collections.Generic;
    using System.Net.Mime;
    using Newtonsoft.Json.Linq;


    /// <summary>
    /// Used to serialize an existing deserialized message when a message is forwarded, scheduled, etc.
    /// </summary>
    public class NewtonsoftXmlBodyMessageSerializer :
        IMessageSerializer
    {
        readonly JsonMessageEnvelope _envelope;

        public NewtonsoftXmlBodyMessageSerializer(MessageEnvelope envelope, ContentType contentType)
        {
            _envelope = new JsonMessageEnvelope(envelope);
            ContentType = contentType;
        }

        public ContentType ContentType { get; }

        public MessageBody GetMessageBody<T>(SendContext<T> context)
            where T : class
        {
            _envelope.DestinationAddress = context.DestinationAddress?.ToString();

            foreach (KeyValuePair<string, object> header in context.Headers.GetAll())
                _envelope.Headers[header.Key] = header.Value;

            return new NewtonsoftXmlMessageBody<T>(context, _envelope);
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
