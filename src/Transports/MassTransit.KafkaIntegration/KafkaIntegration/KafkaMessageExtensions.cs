namespace MassTransit.KafkaIntegration
{
    using System;
    using Confluent.Kafka;


    static class KafkaMessageExtensions
    {
        public static TKey DeserializeKey<TKey>(this IDeserializer<TKey> deserializer, ConsumeResult<byte[], byte[]> result)
        {
            return Deserialize(deserializer, result, result.Message.Key, MessageComponentType.Key);
        }

        public static TValue DeserializeValue<TValue>(this IDeserializer<TValue> deserializer, ConsumeResult<byte[], byte[]> result)
        {
            return Deserialize(deserializer, result, result.Message.Value, MessageComponentType.Value);
        }

        static T Deserialize<T>(IDeserializer<T> deserializer, ConsumeResult<byte[], byte[]> result, byte[] value, MessageComponentType componentType)
        {
            ReadOnlySpan<byte> bytes = value?.Length > 0 ? new ReadOnlySpan<byte>(value) : ReadOnlySpan<byte>.Empty;

            return deserializer.Deserialize(bytes, bytes.IsEmpty, new SerializationContext(componentType, result.Topic, result.Message.Headers));
        }
    }
}
