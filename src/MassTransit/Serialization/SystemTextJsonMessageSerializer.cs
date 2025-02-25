#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Text.Encodings.Web;
    using System.Text.Json;
    using System.Text.Json.Serialization.Metadata;
    using Initializers;
    using Initializers.TypeConverters;
    using JsonConverters;


    public class SystemTextJsonMessageSerializer :
        IMessageDeserializer,
        IMessageSerializer,
        IObjectDeserializer
    {
        public static readonly ContentType JsonContentType = new ContentType("application/vnd.masstransit+json");

        public static JsonSerializerOptions Options;

        public static readonly SystemTextJsonMessageSerializer Instance = new SystemTextJsonMessageSerializer();

        static SystemTextJsonMessageSerializer()
        {
            GlobalTopology.MarkMessageTypeNotConsumable(typeof(JsonElement));

            Options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReadCommentHandling = JsonCommentHandling.Skip,
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,

            #if NET8_0_OR_GREATER
                // Set the TypeInfoResolver property based on whether reflection-based is enabled.
                // If reflection is enabled, combine the default resolver (reflection-based) context with the custom serializer context
                // Otherwise, use only the custom serializer context.
                // User can overwrite it directly or by modifying the TypeInfoResolverChain.
                TypeInfoResolver = JsonSerializer.IsReflectionEnabledByDefault
                    ? JsonTypeInfoResolver.Combine(SystemTextJsonSerializationContext.Default, new DefaultJsonTypeInfoResolver())
                    : SystemTextJsonSerializationContext.Default
            #endif
            };

            Options.Converters.Add(new StringDecimalJsonConverter());
            Options.Converters.Add(new SystemTextJsonMessageDataConverter());
            Options.Converters.Add(new SystemTextJsonConverterFactory());
        }

        public SystemTextJsonMessageSerializer(ContentType? contentType = null)
        {
            ContentType = contentType ?? JsonContentType;
        }

        public ContentType ContentType { get; }

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
                JsonElement? bodyElement = body is JsonMessageBody jsonMessageBody
                    ? jsonMessageBody.GetJsonElement(Options)
                    : JsonSerializer.Deserialize<JsonElement>(body.GetBytes(), Options);

                var envelope = bodyElement?.Deserialize<MessageEnvelope>(Options);
                if (envelope == null)
                    throw new SerializationException("Message envelope not found");

                var messageContext = new EnvelopeMessageContext(envelope, this);

                var messageTypes = envelope.MessageType ?? [];

                return new SystemTextJsonSerializerContext(this, Options, ContentType, messageContext, messageTypes, envelope);
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
            return new SystemTextJsonMessageBody<T>(context, Options);
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
                case string text when string.IsNullOrWhiteSpace(text):
                    return defaultValue;
                case string text when TypeConverterCache.TryGetTypeConverter(out ITypeConverter<T, string>? typeConverter)
                    && typeConverter.TryConvert(text, out var result):
                    return result;
                case string text:
                    return JsonSerializer.Deserialize<JsonElement>(text, Options).GetObject<T>(Options);
                case JsonElement jsonElement:
                    return jsonElement.GetObject<T>(Options);
            }

            var element = JsonSerializer.SerializeToElement(value, Options);

            return element.ValueKind == JsonValueKind.Null
                ? defaultValue
                : element.GetObject<T>(Options);
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
                case string text when string.IsNullOrWhiteSpace(text):
                    return defaultValue;
                case string text when TypeConverterCache.TryGetTypeConverter(out ITypeConverter<T, string>? typeConverter)
                    && typeConverter.TryConvert(text, out var result):
                    return result;
                case string text:
                    return JsonSerializer.Deserialize<T>(text, Options);
                case JsonElement jsonElement:
                    return jsonElement.Deserialize<T>(Options);
            }

            var element = JsonSerializer.SerializeToElement(value, Options);

            return element.ValueKind == JsonValueKind.Null
                ? defaultValue
                : element.Deserialize<T>(Options);
        }

        public MessageBody SerializeObject(object? value)
        {
            if (value == null)
                return new EmptyMessageBody();

            return new SystemTextJsonObjectMessageBody(value, Options);
        }
    }
}
