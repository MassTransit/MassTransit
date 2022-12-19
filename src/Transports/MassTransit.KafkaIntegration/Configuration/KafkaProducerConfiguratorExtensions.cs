namespace MassTransit
{
    using System.Threading.Tasks;
    using Confluent.Kafka;


    public static class KafkaProducerConfiguratorExtensions
    {
        class AsyncSerializerWrapper<T> :
            IAsyncSerializer<T>
        {
            readonly ISerializer<T> _serializer;

            public AsyncSerializerWrapper(ISerializer<T> serializer)
            {
                _serializer = serializer;
            }

            public Task<byte[]> SerializeAsync(T data, SerializationContext context)
            {
                return Task.FromResult(_serializer.Serialize(data, context));
            }
        }


        public static IAsyncSerializer<T> AsAsyncOverSync<T>(this ISerializer<T> serializer)
        {
            return new AsyncSerializerWrapper<T>(serializer);
        }

        /// <summary>Set the serializer to use to serialize keys.</summary>
        /// <remarks>
        /// If your key serializer throws an exception, this will be
        /// wrapped in a ConsumeException with ErrorCode
        /// Local_KeyDeserialization and thrown by the initiating call to
        /// Consume.
        /// </remarks>
        public static void SetKeySerializer<TKey, TValue>(this IKafkaProducerConfigurator<TKey, TValue> configurator, ISerializer<TKey> serializer)
        {
            configurator.SetKeySerializer(serializer.AsAsyncOverSync());
        }

        /// <summary>
        /// Set the serializer to use to serialize values.
        /// </summary>
        /// <remarks>
        /// If your value serializer throws an exception, this will be
        /// wrapped in a ConsumeException with ErrorCode
        /// Local_ValueDeserialization and thrown by the initiating call to
        /// Consume.
        /// </remarks>
        public static void SetValueSerializer<TKey, TValue>(this IKafkaProducerConfigurator<TKey, TValue> configurator, ISerializer<TValue> serializer)
        {
            configurator.SetValueSerializer(serializer.AsAsyncOverSync());
        }
    }
}
