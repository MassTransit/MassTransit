namespace MassTransit.KafkaIntegration.Serializers
{
    using System.Net.Mime;
    using Confluent.Kafka;


    public interface IKafkaSerializerFactory
    {
        ContentType ContentType { get; }
        IDeserializer<T> GetDeserializer<T>();
        IAsyncSerializer<T> GetSerializer<T>();
    }
}
