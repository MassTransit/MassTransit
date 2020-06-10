namespace MassTransit.KafkaIntegration.Serializers
{
    using System;
    using System.IO;
    using Confluent.Kafka;
    using Courier;
    using Newtonsoft.Json;


    public class MassTransitJsonDeserializer<T> :
        IDeserializer<T>
    {
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            using var stream = new MemoryStream(data.ToArray());
            using var writer = new StreamReader(stream);
            using var reader = new JsonTextReader(writer);
            return SerializerCache.Deserializer.Deserialize<T>(reader);
        }
    }
}
