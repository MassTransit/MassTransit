namespace MassTransit.KafkaIntegration.Serializers
{
    using System;
    using System.Collections.Generic;
    using Confluent.Kafka;


    static class SerializationUtils
    {
        public static class Serializers
        {
            //https://github.com/confluentinc/confluent-kafka-dotnet/blob/fd4b95b420418153aa39d7c94df1e1135c09c07d/src/Confluent.Kafka/Producer.cs#L50
            static readonly ISet<Type> DefaultKeyTypes = new HashSet<Type>
            {
                typeof(Null),
                typeof(int),
                typeof(long),
                typeof(string),
                typeof(float),
                typeof(double),
                typeof(byte[])
            };

            public static bool IsDefaultKeyType<TKey>()
            {
                return DefaultKeyTypes.Contains(typeof(TKey));
            }
        }


        public static class DeSerializers
        {
            //https://github.com/confluentinc/confluent-kafka-dotnet/blob/fd4b95b420418153aa39d7c94df1e1135c09c07d/src/Confluent.Kafka/Consumer.cs#L54
            static readonly ISet<Type> DefaultKeyTypes = new HashSet<Type>
            {
                typeof(Ignore),
                typeof(Null),
                typeof(int),
                typeof(long),
                typeof(string),
                typeof(float),
                typeof(double),
                typeof(byte[])
            };

            public static bool IsDefaultKeyType<TKey>()
            {
                return DefaultKeyTypes.Contains(typeof(TKey));
            }
        }
    }
}
