
namespace MassTransit.Serialization.JsonConverters
{
    using MassTransit.Metadata;
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    internal class HostInfoJsonConverter : JsonConverter<HostInfo>
    {
        public override HostInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<BusHostInfo>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, HostInfo value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}
