namespace MassTransit.KafkaIntegration
{
    using System;
    using Confluent.Kafka;


    static class KafkaMessageExtensions
    {
        public static TKey DeserializeKey<TKey>(this Message<byte[], byte[]> message, string topic, IDeserializer<TKey> deserializer)
        {
            ReadOnlySpan<byte> bytes = message.Key?.Length > 0 ? new ReadOnlySpan<byte>(message.Key) : ReadOnlySpan<byte>.Empty;

            return deserializer.Deserialize(bytes, bytes.IsEmpty, new SerializationContext(MessageComponentType.Key, topic, message.Headers));
        }

        public static TValue DeserializeValue<TValue>(this Message<byte[], byte[]> message, string topic, IDeserializer<TValue> deserializer)
        {
            ReadOnlySpan<byte> bytes = message.Value?.Length > 0 ? new ReadOnlySpan<byte>(message.Value) : ReadOnlySpan<byte>.Empty;

            return deserializer.Deserialize(bytes, bytes.IsEmpty, new SerializationContext(MessageComponentType.Value, topic, message.Headers));
        }
    }
}
