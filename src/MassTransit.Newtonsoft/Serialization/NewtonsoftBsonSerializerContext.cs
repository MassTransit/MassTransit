#nullable enable
namespace MassTransit.Serialization
{
    using System.Net.Mime;
    using Newtonsoft.Json;


    public class NewtonsoftBsonSerializerContext :
        NewtonsoftSerializerContext
    {
        readonly ContentType _contentType;
        readonly MessageEnvelope _envelope;

        public NewtonsoftBsonSerializerContext(JsonSerializer deserializer, IObjectDeserializer objectDeserializer, MessageEnvelope envelope,
            ContentType contentType)
            : base(deserializer, objectDeserializer, new EnvelopeMessageContext(envelope, objectDeserializer), envelope.Message!, envelope.MessageType ?? [])
        {
            _contentType = contentType;
            _envelope = envelope;
        }

        public override IMessageSerializer GetMessageSerializer()
        {
            return new NewtonsoftBsonBodyMessageSerializer(_envelope, _contentType);
        }

        public override IMessageSerializer GetMessageSerializer<T>(MessageEnvelope envelope, T message)
        {
            var serializer = new NewtonsoftBsonBodyMessageSerializer(envelope, _contentType);

            serializer.Overlay(message);

            return serializer;
        }

        public override IMessageSerializer GetMessageSerializer(object message, string[] messageTypes)
        {
            var envelope = new JsonMessageEnvelope(this, message, messageTypes);

            return new NewtonsoftBsonBodyMessageSerializer(envelope, _contentType);
        }
    }
}
