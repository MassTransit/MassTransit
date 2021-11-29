namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;


    public class CaseInsensitiveDictionaryJsonConverter<T, TValue> :
        JsonConverter<T>
        where T : class, IEnumerable<KeyValuePair<string, TValue>>
    {
        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (KeyValuePair<string, TValue> element in value)
            {
                if (element.Key != null)
                    writer.WritePropertyName(element.Key);

                JsonSerializer.Serialize(writer, element.Value, options);
            }

            writer.WriteEndObject();
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return ReadInternal(ref reader, typeToConvert, options);
        }

        protected T ReadInternal(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException($"Expected StartObject, found: {reader.TokenType}");

            var dictionary = new Dictionary<string, TValue>(StringComparer.OrdinalIgnoreCase);
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return dictionary as T;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException($"Expected PropertyName, found: {reader.TokenType}");

                var propertyName = reader.GetString();

                if (string.IsNullOrWhiteSpace(propertyName))
                    throw new JsonException("Expected non-empty PropertyName");

                reader.Read();

                dictionary.Add(propertyName, JsonSerializer.Deserialize<TValue>(ref reader, options));
            }

            return dictionary as T;
        }
    }
}
