namespace MassTransit.Serialization.JsonConverters
{
    using System;
    using Metadata;
    using Newtonsoft.Json;


    public class JsonMessageContextHostInfoConverter
        : JsonConverter
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(HostInfo);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, typeof(BusHostInfo));
        }
    }
}
