namespace MassTransit.KafkaIntegration.Serializers
{
    using System.Text.Json;
    using Confluent.Kafka;
    using Serialization;


    public class MassTransitJsonSerializer<T> :
        ISerializer<T>
    {
        public byte[] Serialize(T data, SerializationContext context)
        {
            return JsonSerializer.SerializeToUtf8Bytes(data, SystemTextJsonMessageSerializer.Options);
        }
    }
}
