namespace MassTransit.KafkaIntegration.Serializers
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;


    static class DeserializerTypes
    {
        static readonly Dictionary<Type, object> _defaultDeserializers = new Dictionary<Type, object>
        {
            [typeof(Null)] = Deserializers.Null,
            [typeof(Ignore)] = Deserializers.Ignore,
            [typeof(int)] = Deserializers.Int32,
            [typeof(long)] = Deserializers.Int64,
            [typeof(string)] = Deserializers.Utf8,
            [typeof(float)] = Deserializers.Single,
            [typeof(double)] = Deserializers.Double,
            [typeof(byte[])] = Deserializers.ByteArray
        };

        public static IDeserializer<T> TryGet<T>()
        {
            return _defaultDeserializers.TryGetValue(typeof(T), out var deserializer) ? (IDeserializer<T>)deserializer : null;
        }
    }
}
