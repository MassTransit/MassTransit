namespace MassTransit.Serialization
{
    using System;
    using System.Net.Mime;
    using Newtonsoft.Json;


    public class NewtonsoftEncryptedV2SerializerContext :
        NewtonsoftSerializerContext
    {
        readonly ContentType _contentType;
        readonly ICryptoStreamProviderV2 _cryptoStreamProvider;
        readonly MessageEnvelope _envelope;

        public NewtonsoftEncryptedV2SerializerContext(JsonSerializer deserializer, IObjectDeserializer objectDeserializer, MessageEnvelope envelope,
            ContentType contentType, ICryptoStreamProviderV2 cryptoStreamProvider)
            : base(deserializer, objectDeserializer, new EnvelopeMessageContext(envelope, objectDeserializer), envelope.Message,
                envelope.MessageType ?? Array.Empty<string>())
        {
            _envelope = envelope;
            _contentType = contentType;
            _cryptoStreamProvider = cryptoStreamProvider;
        }

        public override IMessageSerializer GetMessageSerializer()
        {
            return new NewtonsoftEncryptedV2BodyMessageSerializer(_envelope, _contentType, _cryptoStreamProvider);
        }

        public override IMessageSerializer GetMessageSerializer<T>(MessageEnvelope envelope, T message)
        {
            var serializer = new NewtonsoftEncryptedV2BodyMessageSerializer(envelope, _contentType, _cryptoStreamProvider);

            serializer.Overlay(message);

            return serializer;
        }

        public override IMessageSerializer GetMessageSerializer(object message, string[] messageTypes)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var envelope = new JsonMessageEnvelope(this, message, messageTypes);

            return new NewtonsoftEncryptedV2BodyMessageSerializer(envelope, _contentType, _cryptoStreamProvider);
        }
    }
}
