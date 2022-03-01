#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Net.Mime;
    using System.Text.Json;
    using Util;


    public class SystemTextJsonSerializerContext :
        BaseSerializerContext
    {
        readonly ContentType _contentType;
        readonly MessageEnvelope? _envelope;
        readonly object _message;
        readonly JsonSerializerOptions _options;

        public SystemTextJsonSerializerContext(IObjectDeserializer objectDeserializer, JsonSerializerOptions options,
            ContentType contentType, MessageContext messageContext, string[] messageTypes, MessageEnvelope? envelope = null,
            object? message = null)
            : base(objectDeserializer, messageContext, messageTypes)
        {
            _envelope = envelope;
            _contentType = contentType;
            _message = message ?? envelope?.Message ?? throw new ArgumentNullException(nameof(envelope));
            _options = options;
        }

        public override bool TryGetMessage<T>(out T? message)
            where T : class
        {
            var jsonElement = GetJsonElement(_message);

            if (typeof(T) == typeof(JsonElement))
            {
                message = jsonElement as T;
                return message != null;
            }

            if (IsSupportedMessageType<T>())
            {
                message = jsonElement.Deserialize<T>(_options);
                return message != null;
            }

            message = null;
            return false;
        }

        public override bool TryGetMessage(Type messageType, out object? message)
        {
            var jsonElement = GetJsonElement(_message);

            message = jsonElement.Deserialize(messageType, _options);

            return message != null;
        }

        public override IMessageSerializer GetMessageSerializer()
        {
            return _envelope != null
                ? new SystemTextJsonBodyMessageSerializer(_envelope, _contentType, _options)
                : new SystemTextJsonBodyMessageSerializer(_message, _contentType, _options);
        }

        public override IMessageSerializer GetMessageSerializer<T>(MessageEnvelope envelope, T message)
        {
            var serializer = new SystemTextJsonBodyMessageSerializer(envelope, _contentType, _options);

            serializer.Overlay(message);

            return serializer;
        }

        public override IMessageSerializer GetMessageSerializer(object message, string[] messageTypes)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var envelope = new JsonMessageEnvelope(this, message, messageTypes);

            return new SystemTextJsonBodyMessageSerializer(envelope, _contentType, _options);
        }

        public override Dictionary<string, object> ToDictionary<T>(T? message)
            where T : class
        {
            return message == null
                ? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                : JsonSerializer.SerializeToElement(message, _options).Deserialize<Dictionary<string, object>>()!;
        }

        static JsonElement GetJsonElement(object message)
        {
            return message is JsonElement element
                ? element.ValueKind == JsonValueKind.Null
                    ? new JsonElement()
                    : element
                : new JsonElement();
        }
    }
}
