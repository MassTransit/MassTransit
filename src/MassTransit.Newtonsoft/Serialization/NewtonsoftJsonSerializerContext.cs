#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.Net.Mime;
    using Newtonsoft.Json;


    public class NewtonsoftJsonSerializerContext :
        NewtonsoftSerializerContext
    {
        readonly ContentType _contentType;
        readonly MessageEnvelope _envelope;

        public NewtonsoftJsonSerializerContext(JsonSerializer deserializer, IObjectDeserializer objectDeserializer, MessageEnvelope envelope,
            ContentType contentType)
            : base(deserializer, objectDeserializer, new EnvelopeMessageContext(envelope, objectDeserializer), envelope.Message,
                envelope.MessageType ?? Array.Empty<string>())
        {
            _contentType = contentType;
            _envelope = envelope;
        }

        public override IMessageSerializer GetMessageSerializer()
        {
            return new NewtonsoftJsonBodyMessageSerializer(_envelope, _contentType);
        }

        public override IMessageSerializer GetMessageSerializer<T>(MessageEnvelope envelope, T message)
        {
            var serializer = new NewtonsoftJsonBodyMessageSerializer(envelope, _contentType);

            serializer.Overlay(message);

            return serializer;
        }

        public override IMessageSerializer GetMessageSerializer(object message, string[] messageTypes)
        {
            var envelope = new JsonMessageEnvelope(this, message, messageTypes);

            return new NewtonsoftJsonBodyMessageSerializer(envelope, _contentType);
        }
    }
}
