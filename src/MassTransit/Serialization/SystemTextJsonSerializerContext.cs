#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Mime;
    using System.Text.Json;
    using System.Text.Json.Nodes;


    public class SystemTextJsonSerializerContext :
        BaseSerializerContext
    {
        readonly MessageEnvelope? _envelope;

        public SystemTextJsonSerializerContext(IObjectDeserializer objectDeserializer, JsonSerializerOptions options, ContentType contentType,
            MessageContext messageContext, string[] messageTypes, MessageEnvelope? envelope = null, object? message = null)
            : base(objectDeserializer, messageContext, messageTypes)
        {
            _envelope = envelope;
            ContentType = contentType;
            Message = message ?? envelope?.Message ?? throw new ArgumentNullException(nameof(envelope));
            Options = options;
        }

        protected object Message { get; }
        protected ContentType ContentType { get; }
        protected JsonSerializerOptions Options { get; }

        public override bool TryGetMessage<T>(out T? message)
            where T : class
        {
            var jsonElement = GetJsonElement(Message);

            if (typeof(T) == typeof(JsonObject))
            {
                message = JsonObject.Create(jsonElement) as T;
                return message != null;
            }

            if (IsSupportedMessageType<T>())
            {
                if (Message is T messageOfT)
                {
                    message = messageOfT;
                    return true;
                }

                message = jsonElement.Deserialize<T>(Options);
                return message != null;
            }

            message = null;
            return false;
        }

        public override bool TryGetMessage(Type messageType, [NotNullWhen(true)] out object? message)
        {
            var jsonElement = GetJsonElement(Message);

            message = jsonElement.Deserialize(messageType, Options);

            return message != null;
        }

        public override IMessageSerializer GetMessageSerializer()
        {
            if (_envelope == null)
                throw new InvalidOperationException("This should be overloaded");

            return new SystemTextJsonBodyMessageSerializer(_envelope, ContentType, Options);
        }

        public override IMessageSerializer GetMessageSerializer<T>(MessageEnvelope envelope, T message)
        {
            var serializer = new SystemTextJsonBodyMessageSerializer(envelope, ContentType, Options);

            serializer.Overlay(message);

            return serializer;
        }

        public override IMessageSerializer GetMessageSerializer(object message, string[] messageTypes)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var envelope = new JsonMessageEnvelope(this, message, messageTypes);

            return new SystemTextJsonBodyMessageSerializer(envelope, ContentType, Options, messageTypes);
        }

        public override Dictionary<string, object> ToDictionary<T>(T? message)
            where T : class
        {
            return message == null
                ? new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                : JsonSerializer.SerializeToElement(message, Options).Deserialize<Dictionary<string, object>>()!;
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
