namespace MassTransit.KafkaIntegration.Serializers
{
    using System;
    using System.IO;
    using Confluent.Kafka;
    using Courier;
    using Newtonsoft.Json;


    public class MassTransitSerializer<T> :
        ISerializer<T>,
        IDeserializer<T>
    {
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            using var stream = new MemoryStream(data.ToArray());
            using var writer = new StreamReader(stream);
            using var reader = new JsonTextReader(writer);
            return SerializerCache.Deserializer.Deserialize<T>(reader);
        }

        public byte[] Serialize(T data, SerializationContext context)
        {
            using var stream = new MemoryStream();
            using var writer = new StreamWriter(stream);
            SerializerCache.Serializer.Serialize(writer, data);

            writer.Flush();
            return stream.ToArray();
        }
    }
}
