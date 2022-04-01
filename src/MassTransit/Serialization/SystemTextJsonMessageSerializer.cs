#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.Net.Mime;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text.Encodings.Web;
    using System.Text.Json;
    using Initializers;
    using Initializers.TypeConverters;
    using JsonConverters;
    using Metadata;


    public class SystemTextJsonMessageSerializer :
        IMessageDeserializer,
        IMessageSerializer,
        IObjectDeserializer
    {
        public static readonly ContentType JsonContentType = new ContentType("application/vnd.masstransit+json");

        public static JsonSerializerOptions DefaultOptions { get; }

        [Obsolete("This only exists for backwards compatibility")]
        public static JsonSerializerOptions Options;

        public static readonly SystemTextJsonMessageSerializer Instance = new SystemTextJsonMessageSerializer();

        static SystemTextJsonMessageSerializer()
        {
            GlobalTopology.MarkMessageTypeNotConsumable(typeof(JsonElement));

            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReadCommentHandling = JsonCommentHandling.Skip,
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };

            options.Converters.Add(new SystemTextJsonMessageDataConverter());
            options.Converters.Add(new SystemTextJsonConverterFactory());

            // Seal the options - preventing further modifications
            JsonSerializer.Serialize(true, options);

            DefaultOptions = options;

            #pragma warning disable CS0618
            Options = DefaultOptions;
            #pragma warning restore CS0618
        }

        readonly JsonSerializerOptions? _options;

        public SystemTextJsonMessageSerializer(ContentType? contentType = null)
        {
            ContentType = contentType ?? JsonContentType;
        }

        public SystemTextJsonMessageSerializer(ContentType? contentType, JsonSerializerOptions? options = null)
            : this(contentType)
        {
            _options = options;
        }

        public ContentType ContentType { get; }

        public JsonSerializerOptions SerializerOptions
            #pragma warning disable CS0618
            => _options ?? Options;
            #pragma warning restore CS0618

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("json");
            scope.Add("contentType", ContentType.MediaType);
            scope.Add("provider", "System.Text.Json");
        }

        public ConsumeContext Deserialize(ReceiveContext receiveContext)
        {
            return new BodyConsumeContext(receiveContext, Deserialize(receiveContext.Body, receiveContext.TransportHeaders, receiveContext.InputAddress));
        }

        public SerializerContext Deserialize(MessageBody body, Headers headers, Uri? destinationAddress = null)
        {
            try
            {
                var envelope = JsonSerializer.Deserialize<MessageEnvelope>(body.GetBytes(), SerializerOptions);
                if (envelope == null)
                    throw new SerializationException("Message envelope not found");

                var messageContext = new EnvelopeMessageContext(envelope, this);

                var messageTypes = envelope.MessageType ?? Array.Empty<string>();

                var serializerContext = new SystemTextJsonSerializerContext(this, SerializerOptions, ContentType, messageContext, messageTypes, envelope);

                return serializerContext;
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException("An error occured while deserializing the message envelope", ex);
            }
        }

        public MessageBody GetMessageBody(string text)
        {
            return new StringMessageBody(text);
        }

        public MessageBody GetMessageBody<T>(SendContext<T> context)
            where T : class
        {
            return new SystemTextJsonMessageBody<T>(context, SerializerOptions);
        }

        public T? DeserializeObject<T>(object? value, T? defaultValue = default)
            where T : class
        {
            switch (value)
            {
                case null:
                    return defaultValue;
                case T returnValue:
                    return returnValue;
                case string text:
                    if (TypeConverterCache.TryGetTypeConverter(out ITypeConverter<T, string>? typeConverter) && typeConverter.TryConvert(text, out var result))
                        return result;
                    return GetObject<T>(JsonSerializer.Deserialize<JsonElement>(text), SerializerOptions);
                case JsonElement jsonElement:
                    return GetObject<T>(jsonElement, SerializerOptions);
            }

            var element = JsonSerializer.SerializeToElement(value, SerializerOptions);

            return element.ValueKind == JsonValueKind.Null
                ? defaultValue
                : GetObject<T>(element, SerializerOptions);
        }

        public T? DeserializeObject<T>(object? value, T? defaultValue = null)
            where T : struct
        {
            switch (value)
            {
                case null:
                    return defaultValue;
                case T returnValue:
                    return returnValue;
                case string text:
                    if (TypeConverterCache.TryGetTypeConverter(out ITypeConverter<T, string>? typeConverter) && typeConverter.TryConvert(text, out var result))
                        return result;
                    return JsonSerializer.Deserialize<T>(text, SerializerOptions);
                case JsonElement jsonElement:
                    return jsonElement.Deserialize<T>(SerializerOptions);
            }

            var element = JsonSerializer.SerializeToElement(value, SerializerOptions);

            return element.ValueKind == JsonValueKind.Null
                ? defaultValue
                : element.Deserialize<T>(SerializerOptions);
        }

        static T? GetObject<T>(JsonElement jsonElement, JsonSerializerOptions options)
            where T : class
        {
            if (typeof(T).GetTypeInfo().IsInterface && MessageTypeCache<T>.IsValidMessageType)
            {
                var messageType = TypeMetadataCache<T>.ImplementationType;

                if (jsonElement.Deserialize(messageType, options) is T obj)
                    return obj;
            }

            return jsonElement.Deserialize<T>(options);
        }
    }
}
