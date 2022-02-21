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
            };

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
                var envelope = JsonSerializer.Deserialize<MessageEnvelope>(body.GetBytes(), Options);
                if (envelope == null)
                    throw new SerializationException("Message envelope not found");

                var messageContext = new EnvelopeMessageContext(envelope, this);

                var messageTypes = envelope.MessageType ?? Array.Empty<string>();

                var serializerContext = new SystemTextJsonSerializerContext(this, Options, ContentType, messageContext, messageTypes, envelope);

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
                case string text:
                    if (TypeConverterCache.TryGetTypeConverter(out ITypeConverter<T, string>? typeConverter) && typeConverter.TryConvert(text, out var result))
                        return result;
                    return GetObject<T>(JsonSerializer.Deserialize<JsonElement>(text));
                case JsonElement jsonElement:
                    return GetObject<T>(jsonElement);
            }

            var element = JsonSerializer.SerializeToElement(value, Options);

            return element.ValueKind == JsonValueKind.Null
                ? defaultValue
                : GetObject<T>(element);
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
                    return JsonSerializer.Deserialize<T>(text, Options);
                case JsonElement jsonElement:
                    return jsonElement.Deserialize<T>(Options);
            }

            var element = JsonSerializer.SerializeToElement(value, Options);

            return element.ValueKind == JsonValueKind.Null
                ? defaultValue
                : element.Deserialize<T>(Options);
        }

        static T? GetObject<T>(JsonElement jsonElement)
            where T : class
        {
            if (typeof(T).GetTypeInfo().IsInterface && MessageTypeCache<T>.IsValidMessageType)
            {
                var messageType = TypeMetadataCache<T>.ImplementationType;

                if (jsonElement.Deserialize(messageType, Options) is T obj)
                    return obj;
            }

            return jsonElement.Deserialize<T>(Options);
        }
    }
}
