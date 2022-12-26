namespace MassTransit.KafkaIntegration.Serializers
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;


    static class SerializerTypes
    {
        static readonly Dictionary<Type, object> _defaultSerializers = new Dictionary<Type, object>
        {
            [typeof(Null)] = Serializers.Null,
            [typeof(int)] = Serializers.Int32,
            [typeof(long)] = Serializers.Int64,
            [typeof(string)] = Serializers.Utf8,
            [typeof(float)] = Serializers.Single,
            [typeof(double)] = Serializers.Double,
            [typeof(byte[])] = Serializers.ByteArray
        };

        public static IAsyncSerializer<T> TryGet<T>()
        {
            return _defaultSerializers.TryGetValue(typeof(T), out var deserializer) ? ((ISerializer<T>)deserializer).AsAsyncOverSync() : null;
        }
    }
}
