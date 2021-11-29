#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.Net.Mime;
    using Newtonsoft.Json;


    public class NewtonsoftEncryptedSerializerContext :
        NewtonsoftSerializerContext
    {
        readonly ContentType _contentType;
        readonly ICryptoStreamProvider _cryptoStreamProvider;
        readonly MessageEnvelope _envelope;

        public NewtonsoftEncryptedSerializerContext(JsonSerializer deserializer, IObjectDeserializer objectDeserializer, MessageEnvelope envelope,
            ContentType contentType, ICryptoStreamProvider cryptoStreamProvider)
            : base(deserializer, objectDeserializer, new EnvelopeMessageContext(envelope, objectDeserializer), envelope.Message,
                envelope.MessageType ?? Array.Empty<string>())
        {
            _contentType = contentType;
            _cryptoStreamProvider = cryptoStreamProvider;
            _envelope = envelope;
        }

        public override IMessageSerializer GetMessageSerializer()
        {
            return new NewtonsoftEncryptedBodyMessageSerializer(_envelope, _contentType, _cryptoStreamProvider);
        }

        public override IMessageSerializer GetMessageSerializer<T>(MessageEnvelope envelope, T message)
        {
            var serializer = new NewtonsoftEncryptedBodyMessageSerializer(envelope, _contentType, _cryptoStreamProvider);

            serializer.Overlay(message);

            return serializer;
        }

        public override IMessageSerializer GetMessageSerializer(object message, string[] messageTypes)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var envelope = new JsonMessageEnvelope(this, message, messageTypes);

            return new NewtonsoftEncryptedBodyMessageSerializer(envelope, _contentType, _cryptoStreamProvider);
        }
    }
}
