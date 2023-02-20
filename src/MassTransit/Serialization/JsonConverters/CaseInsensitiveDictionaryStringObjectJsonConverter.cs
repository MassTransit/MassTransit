namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.Json;
    using System.Text.Json.Serialization;


    public class CaseInsensitiveDictionaryStringObjectJsonConverter<T> :
        JsonConverter<T>
        where T : class, IEnumerable<KeyValuePair<string, object>>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase) as T;

                case JsonTokenType.StartObject:
                    return ReadObject(ref reader, options) as T;

                case JsonTokenType.StartArray:
                    return ReadArray(ref reader, options) as T;

                default:
                    throw new JsonException($"Expected StartObject or StartArray, found: {reader.TokenType}");
            }
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            Write(writer, value, options);
        }

        static void Write(Utf8JsonWriter writer, IEnumerable<KeyValuePair<string, object>> values, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (KeyValuePair<string, object> element in values)
                WriteValue(writer, element.Key, element.Value, options);

            writer.WriteEndObject();
        }

        static void WriteValue(Utf8JsonWriter writer, string key, object objectValue, JsonSerializerOptions options)
        {
            if (key == null)
                return;

            var ignoreDefault = options.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingDefault;
            var ignoreNull = ignoreDefault || options.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingNull;

            if (objectValue == null && ignoreNull)
                return;

            switch (objectValue)
            {
                case null:
                    writer.WriteNull(key);
                    break;
                case string value:
                    writer.WriteString(key, value);
                    break;
                case DateTime value:
                    if (value != default)
                        writer.WriteString(key, value);
                    break;
                case DateTimeOffset value:
                    if (value != default)
                        writer.WriteString(key, value);
                    break;
                case Guid value:
                    if (value != default)
                        writer.WriteString(key, value.ToString("D"));
                    break;
                case long value:
                    if (value != default || !ignoreDefault)
                        writer.WriteNumber(key, value);
                    break;
                case int value:
                    if (value != default || !ignoreDefault)
                        writer.WriteNumber(key, value);
                    break;
                case short value:
                    if (value != default || !ignoreDefault)
                        writer.WriteNumber(key, value);
                    break;
                case byte value:
                    if (value != default || !ignoreDefault)
                        writer.WriteNumber(key, value);
                    break;
                case float value:
                    writer.WriteNumber(key, value);
                    break;
                case double value:
                    writer.WriteNumber(key, value);
                    break;
                case decimal value:
                    if (value != default || !ignoreDefault)
                    {
                        var text = Convert.ToString(value, CultureInfo.InvariantCulture);
                        if (!string.IsNullOrWhiteSpace(text))
                            writer.WriteString(key, text);
                    }

                    break;
                case bool value:
                    if (value || !ignoreDefault)
                        writer.WriteBoolean(key, value);
                    break;

                default:
                    writer.WritePropertyName(key);
                    JsonSerializer.Serialize(writer, objectValue, options);
                    break;
            }
        }

        static Dictionary<string, object> ReadObject(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var ignoreDefault = options.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingDefault;
            var ignoreNull = ignoreDefault || options.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingNull;

            var dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return dictionary;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException($"Expected PropertyName, found: {reader.TokenType}");

                var propertyName = reader.GetString();

                if (string.IsNullOrWhiteSpace(propertyName))
                    throw new JsonException("Expected non-empty PropertyName");

                reader.Read();

                var value = ReadPropertyValue(ref reader, options);
                if (value != null || !ignoreNull)
                    dictionary[propertyName] = value;
            }

            return dictionary;
        }

        static Dictionary<string, object> ReadArray(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var ignoreDefault = options.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingDefault;
            var ignoreNull = ignoreDefault || options.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingNull;

            var dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    return dictionary;

                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    Dictionary<string, object> elementDictionary = ReadObject(ref reader, options);
                    if (elementDictionary.TryGetValue("Key", out string key) && !string.IsNullOrWhiteSpace(key)
                        && elementDictionary.TryGetValue("Value", out var value))
                    {
                        if (value != null || !ignoreNull)
                            dictionary[key] = value;
                    }
                }
                else
                    throw new JsonException($"Expected object (key/value), found: {reader.TokenType}");
            }

            return dictionary;
        }

        static object ReadPropertyValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    return reader.GetString();

                case JsonTokenType.False:
                    return false;

                case JsonTokenType.True:
                    return true;

                case JsonTokenType.Null:
                    return null;

                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out var result))
                        return result;

                    return reader.GetDouble();

                case JsonTokenType.StartObject:
                    return ReadObject(ref reader, options);

                case JsonTokenType.StartArray:
                    var list = new List<object>();
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                        list.Add(ReadPropertyValue(ref reader, options));
                    return list;

                default:
                    throw new JsonException($"Unsupported JsonTokenType found: {reader.TokenType}");
            }
        }
    }
}
