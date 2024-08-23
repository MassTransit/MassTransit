namespace MassTransit.Serialization.JsonConverters;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;


public class CustomMessageTypeJsonConverter<T> :
    JsonConverter<T>
    where T : class
{
    readonly JsonSerializerOptions _options;

    public CustomMessageTypeJsonConverter(JsonSerializerOptions options)
    {
        _options = options;
    }

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<T>(ref reader, _options);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, _options);
    }
}
