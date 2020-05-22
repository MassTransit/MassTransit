namespace MassTransit.KafkaIntegration.Serializers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Confluent.Kafka;
    using Context;
    using Courier;
    using Newtonsoft.Json.Linq;
    using Transports;


    public static class DictionaryHeadersSerialize
    {
        static readonly Encoding _encoding = Encoding.UTF8;
        static readonly Lazy<IHeadersDeserializer> _deserializer = new Lazy<IHeadersDeserializer>(() => new DictionaryHeadersDeserializer());
        static readonly Lazy<IHeadersSerializer> _serializer = new Lazy<IHeadersSerializer>(() => new DictionaryHeadersSerializer());
        public static IHeadersDeserializer Deserializer => _deserializer.Value;
        public static IHeadersSerializer Serializer => _serializer.Value;


        class DictionaryHeadersDeserializer :
            IHeadersDeserializer
        {
            public IHeaderProvider Deserialize(Headers headers)
            {
                Dictionary<string, object> dictionary = headers.ToDictionary(x => x.Key, x => (object)_encoding.GetString(x.GetValueBytes()));
                return new DictionaryHeaderProvider(dictionary);
            }
        }


        class DictionaryHeadersSerializer :
            IHeadersSerializer
        {
            public Headers Serialize(Dictionary<string, object> headers)
            {
                var result = new Headers();
                foreach (KeyValuePair<string, object> kv in headers)
                    result.Add(kv.Key, Serialize(kv.Value));

                return result;
            }

            static byte[] Serialize(object value)
            {
                using var jsonWriter = new JTokenWriter();
                jsonWriter.WriteValue(value);
                SerializerCache.Serializer.Serialize(jsonWriter, value, value.GetType());
                return _encoding.GetBytes(jsonWriter.Token.ToString());
            }
        }
    }
}
