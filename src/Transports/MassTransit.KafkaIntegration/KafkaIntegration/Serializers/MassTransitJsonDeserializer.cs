namespace MassTransit.KafkaIntegration.Serializers
{
    using System;
    using System.Text.Json;
    using Confluent.Kafka;
    using Serialization;


    public class MassTransitJsonDeserializer<T> :
        IDeserializer<T>
    {
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            return JsonSerializer.Deserialize<T>(data, SystemTextJsonMessageSerializer.Options);
        }
    }
}
