namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using System.Collections.Generic;
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
            if (key != null)
                writer.WritePropertyName(key);

            switch (objectValue)
            {
                case string stringValue:
                    writer.WriteStringValue(stringValue);
                    break;
                case DateTime dateTime:
                    writer.WriteStringValue(dateTime);
                    break;
                case DateTimeOffset dateTime:
                    writer.WriteStringValue(dateTime);
                    break;
                case Guid guid:
                    writer.WriteStringValue(guid.ToString("D"));
                    break;
                case long longValue:
                    writer.WriteNumberValue(longValue);
                    break;
                case int intValue:
                    writer.WriteNumberValue(intValue);
                    break;
                case short shortValue:
                    writer.WriteNumberValue(shortValue);
                    break;
                case byte byteValue:
                    writer.WriteNumberValue(byteValue);
                    break;
                case float floatValue:
                    writer.WriteNumberValue(floatValue);
                    break;
                case double doubleValue:
                    writer.WriteNumberValue(doubleValue);
                    break;
                case decimal decimalValue:
                    writer.WriteNumberValue(decimalValue);
                    break;
                case bool boolValue:
                    writer.WriteBooleanValue(boolValue);
                    break;

                default:
                    JsonSerializer.Serialize(writer, objectValue, options);
                    break;

                // case IEnumerable<KeyValuePair<string, object>> enumerable:
                //     Write(writer, enumerable, options);
                //     break;
                // case object[] array:
                //     writer.WriteStartArray();
                //     foreach (var item in array)
                //     {
                //         HandleValue(writer, item);
                //     }
                //
                //     writer.WriteEndArray();
                //     break;
                // default:
                //     writer.WriteNullValue();
                //     break;
            }
        }

        static Dictionary<string, object> ReadObject(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
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

                dictionary.Add(propertyName, ReadPropertyValue(ref reader, options));
            }

            return dictionary;
        }

        static Dictionary<string, object> ReadArray(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    return dictionary;

                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    Dictionary<string, object> elementDictionary = ReadObject(ref reader, options);
                    if (elementDictionary.TryGetValue("Key", out string key) && elementDictionary.TryGetValue("Value", out var value))
                        dictionary.Add(key, value);
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
                    if (reader.TryGetDateTime(out var date))
                        return date;
                    if (reader.TryGetGuid(out var guid))
                        return guid;

                    var text = reader.GetString();

                    // if (Uri.IsWellFormedUriString(text, UriKind.Absolute))
                    //     return new Uri(text);
                    //
                    return text;

                case JsonTokenType.False:
                    return false;

                case JsonTokenType.True:
                    return true;

                case JsonTokenType.Null:
                    return null;

                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out var result))
                        return result;

                    return reader.GetDecimal();

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
