namespace MassTransit.Serialization
{
    using System;
    using System.Net.Mime;
    using Newtonsoft.Json;


    public class NewtonsoftXmlSerializerContext :
        NewtonsoftSerializerContext
    {
        readonly ContentType _contentType;
        readonly MessageEnvelope _envelope;

        public NewtonsoftXmlSerializerContext(JsonSerializer deserializer, IObjectDeserializer objectDeserializer, MessageEnvelope envelope,
            ContentType contentType)
            : base(deserializer, objectDeserializer, new EnvelopeMessageContext(envelope, objectDeserializer), envelope.Message,
                envelope.MessageType ?? Array.Empty<string>())
        {
            _envelope = envelope;
            _contentType = contentType;
        }

        public override IMessageSerializer GetMessageSerializer()
        {
            return new NewtonsoftXmlBodyMessageSerializer(_envelope, _contentType);
        }

        public override IMessageSerializer GetMessageSerializer<T>(MessageEnvelope envelope, T message)
        {
            var serializer = new NewtonsoftXmlBodyMessageSerializer(envelope, _contentType);

            serializer.Overlay(message);

            return serializer;
        }

        public override IMessageSerializer GetMessageSerializer(object message, string[] messageTypes)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var envelope = new JsonMessageEnvelope(this, message, messageTypes);

            return new NewtonsoftXmlBodyMessageSerializer(envelope, _contentType);
        }
    }
}
