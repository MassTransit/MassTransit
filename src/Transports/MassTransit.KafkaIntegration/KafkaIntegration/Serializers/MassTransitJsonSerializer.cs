namespace MassTransit.KafkaIntegration.Serializers
{
    using System.Text.Json;
    using System.Threading.Tasks;
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


    public class MassTransitAsyncJsonSerializer<T> :
        IAsyncSerializer<T>
    {
        public Task<byte[]> SerializeAsync(T data, SerializationContext context)
        {
            return Task.FromResult(JsonSerializer.SerializeToUtf8Bytes(data, SystemTextJsonMessageSerializer.Options));
        }
    }
}
