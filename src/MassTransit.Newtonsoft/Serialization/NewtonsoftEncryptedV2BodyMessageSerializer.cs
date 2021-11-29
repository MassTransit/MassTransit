namespace MassTransit.Serialization
{
    using System.Net.Mime;
    using Newtonsoft.Json.Linq;


    /// <summary>
    /// Used to serialize an existing deserialized message when a message is forwarded, scheduled, etc.
    /// </summary>
    public class NewtonsoftEncryptedV2BodyMessageSerializer :
        IMessageSerializer
    {
        readonly JsonMessageEnvelope _envelope;
        readonly ICryptoStreamProviderV2 _provider;

        public NewtonsoftEncryptedV2BodyMessageSerializer(MessageEnvelope envelope, ContentType contentType, ICryptoStreamProviderV2 provider)
        {
            _provider = provider;
            _envelope = new JsonMessageEnvelope(envelope);
            ContentType = contentType;
        }

        public ContentType ContentType { get; }

        public MessageBody GetMessageBody<T>(SendContext<T> context)
            where T : class
        {
            _envelope.Update(context);

            var clearMessageBody = new NewtonsoftBsonMessageBody<T>(context, _envelope);

            return new EncryptMessageBodyV2<T>(_provider, context, clearMessageBody);
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
