#nullable enable
namespace MassTransit.Serialization
{
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
            }

            var token = value as JToken ?? JToken.FromObject(value);
            if (token.Type == JTokenType.Null)
                return defaultValue;

            using var jsonReader = token.CreateReader();
            return _deserializer.Deserialize<T>(jsonReader);
        }
    }
}
