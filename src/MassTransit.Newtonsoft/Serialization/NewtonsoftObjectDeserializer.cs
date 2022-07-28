#nullable enable
namespace MassTransit.Serialization
{
    using Initializers;
    using Initializers.TypeConverters;
    using Metadata;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    public class NewtonsoftObjectDeserializer :
        IObjectDeserializer
    {
        readonly JsonSerializer _deserializer;

        public NewtonsoftObjectDeserializer(JsonSerializer deserializer)
        {
            _deserializer = deserializer;
        }

        public T? DeserializeObject<T>(object? value, T? defaultValue = default)
            where T : class
        {
            switch (value)
            {
                case null:
                    return defaultValue;
                case T headerValue:
                    return headerValue;
                case string text when string.IsNullOrWhiteSpace(text):
                    return defaultValue;
                case string json when typeof(T).IsInterface && TypeMetadataCache<T>.IsValidMessageType:
                    return JsonConvert.DeserializeObject<T>(json, NewtonsoftJsonMessageSerializer.DeserializerSettings);
                case string text when TypeConverterCache.TryGetTypeConverter(out ITypeConverter<T, string>? typeConverter)
                    && typeConverter.TryConvert(text, out var result):
                    return result;
                case string json:
                    return JsonConvert.DeserializeObject<T>(json, NewtonsoftJsonMessageSerializer.DeserializerSettings);
            }

            var token = value as JToken ?? JToken.FromObject(value);
            if (token.Type == JTokenType.Null)
                return defaultValue;

            using var jsonReader = token.CreateReader();
            return _deserializer.Deserialize<T>(jsonReader);
        }

        public T? DeserializeObject<T>(object? value, T? defaultValue = null)
            where T : struct
        {
            switch (value)
            {
                case null:
                    return defaultValue;
                case T headerValue:
                    return headerValue;
                case string text when string.IsNullOrWhiteSpace(text):
                    return defaultValue;
                case string text when TypeConverterCache.TryGetTypeConverter(out ITypeConverter<T, string>? typeConverter)
                    && typeConverter.TryConvert(text, out var result):
                    return result;
                case string json:
                    return JsonConvert.DeserializeObject<T>(json, NewtonsoftJsonMessageSerializer.DeserializerSettings);
            }

            var token = value as JToken ?? JToken.FromObject(value);
            if (token.Type == JTokenType.Null)
                return defaultValue;

            using var jsonReader = token.CreateReader();
            return _deserializer.Deserialize<T>(jsonReader);
        }

        public MessageBody SerializeObject(object? value)
        {
            if (value == null)
                return new EmptyMessageBody();

            return new NewtonsoftJsonObjectMessageBody(value);
        }
    }
}
