namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;


    public class JsonMessageContextHeaderConverter
        : JsonConverter
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Headers);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (existingValue is IDictionary<string, object> dictionary)
                return new JsonEnvelopeHeaders(dictionary, serializer);

            return default;
        }
    }
}
