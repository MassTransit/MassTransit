namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;


    public class TypeMappingJsonConverter<TType, TImplementation> :
        JsonConverter<TType>
        where TImplementation : TType
    {
        public override TType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<TImplementation>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, TType value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (TImplementation)value!, options);
        }
    }
}
