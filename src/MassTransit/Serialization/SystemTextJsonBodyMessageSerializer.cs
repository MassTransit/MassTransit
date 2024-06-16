#nullable enable
namespace MassTransit.Serialization
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mime;
    using System.Text.Json;
    using System.Text.Json.Nodes;


    /// <summary>
    /// Used to serialize an existing deserialized message when a message is forwarded, scheduled, etc.
    /// </summary>
    public class SystemTextJsonBodyMessageSerializer :
        RawMessageSerializer,
        IMessageSerializer
    {
        readonly JsonMessageEnvelope? _envelope;
        readonly string[]? _messageTypes;
        readonly JsonSerializerOptions _options;
        readonly RawSerializerOptions? _rawOptions;
        object? _message;

        public SystemTextJsonBodyMessageSerializer(MessageEnvelope envelope, ContentType contentType, JsonSerializerOptions options,
            string[]? messageTypes = null)
        {
            _message = envelope.Message;
            _options = options;
            _messageTypes = messageTypes;

            _envelope = new JsonMessageEnvelope(envelope);

            ContentType = contentType;
        }

        public SystemTextJsonBodyMessageSerializer(object message, ContentType contentType, JsonSerializerOptions options, RawSerializerOptions rawOptions,
            string[]? messageTypes = null)
        {
            if (message is MessageEnvelope envelope)
            {
                _message = envelope.Message;
                _envelope = new JsonMessageEnvelope(envelope);
            }
            else
                _message = message;

            _options = options;
            _rawOptions = rawOptions;
            _messageTypes = messageTypes;

            ContentType = contentType;
        }

        public ContentType ContentType { get; }

        public MessageBody GetMessageBody<T>(SendContext<T> context)
            where T : class
        {
            _envelope?.Update(context);

            if (_messageTypes != null)
                context.SupportedMessageTypes = _messageTypes;

            if (_rawOptions.HasValue)
            {
                if (_rawOptions.Value.HasFlag(RawSerializerOptions.AddTransportHeaders))
                    SetRawMessageHeaders(context);

                return new SystemTextJsonRawMessageBody<T>(context, _options, _message);
            }

            return new SystemTextJsonMessageBody<T>(context, _options, _envelope);
        }

        public void Overlay(object message)
        {
            if (_message is JsonElement element)
            {
                var overlayElement = JsonSerializer.SerializeToElement(message, _options);
                if ((element.ValueKind == JsonValueKind.Object || element.ValueKind == JsonValueKind.Array) && element.ValueKind == overlayElement.ValueKind)
                    _message = Merge(element, overlayElement);
            }
            else
                _message = message;

            if (_envelope != null)
                _envelope.Message = _message;
        }

        static JsonNode Merge(JsonElement original, JsonElement overlay)
        {
            return original.ValueKind == JsonValueKind.Array
                ? MergeArray(original, overlay)
                : MergeObject(original, overlay);
        }

        static JsonNode MergeObject(JsonElement original, JsonElement overlay)
        {
            var jsonObject = new JsonObject();

            foreach (var property in original.EnumerateObject())
            {
                var name = property.Name;

                if (overlay.TryGetProperty(name, out var overlayElement) && overlayElement.ValueKind != JsonValueKind.Null)
                {
                    jsonObject.Add(name, property.Value.ValueKind switch
                    {
                        JsonValueKind.Object when overlayElement.ValueKind == JsonValueKind.Object => MergeObject(property.Value, overlayElement),
                        JsonValueKind.Array when overlayElement.ValueKind == JsonValueKind.Array => MergeArray(property.Value, overlayElement),
                        _ => ToNode(overlayElement)
                    });
                }
                else
                    jsonObject.Add(name, ToNode(property.Value));
            }

            foreach (var property in overlay.EnumerateObject().Where(property => !original.TryGetProperty(property.Name, out _)))
                jsonObject.Add(property.Name, ToNode(property.Value));

            return jsonObject;
        }

        static JsonNode MergeArray(JsonElement original, JsonElement overlay)
        {
            var jsonArray = new JsonArray();

            foreach (var element in original.EnumerateArray())
                jsonArray.Add(ToNode(element));

            foreach (var element in overlay.EnumerateArray())
                jsonArray.Add(ToNode(element));

            return jsonArray;
        }

        static JsonNode? ToNode(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.Null => null,
                JsonValueKind.Array => new JsonArray(element.EnumerateArray().Select(x => ToNode(x)).ToArray()),
                JsonValueKind.Object => new JsonObject(element.EnumerateObject().Select(x => new KeyValuePair<string, JsonNode?>(x.Name, ToNode(x.Value)))),
                _ => JsonNode.Parse(element.GetRawText())
            };
        }
    }
}
