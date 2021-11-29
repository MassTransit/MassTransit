#nullable enable
namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;


    public class SystemTextJsonUriConverter :
        JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert == typeof(Uri))
                return true;

            return false;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return new UriConverter();
        }


        class UriConverter :
            JsonConverter<Uri>
        {
            public override Uri? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                    return null;

                if (reader.TokenType == JsonTokenType.String)
                {
                    var text = reader.GetString();
                    return string.IsNullOrWhiteSpace(text) ? null : new Uri(text);
                }

                throw new JsonException($"Expected Uri, found: {reader.TokenType}");
            }

            public override void Write(Utf8JsonWriter writer, Uri value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }
        }
    }
}
