namespace MassTransit.Serialization
{
    using System.Text.Json.Serialization;
    using Metadata;


    [JsonSerializable(typeof(MessageEnvelope))]
    [JsonSerializable(typeof(JsonMessageEnvelope))]
    [JsonSerializable(typeof(BusHostInfo))]
    partial class SystemTextJsonSerializationContext :
        JsonSerializerContext
    {
    }
}
