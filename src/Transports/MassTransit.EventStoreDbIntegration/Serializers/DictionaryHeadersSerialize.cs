using System;
using System.Collections.Generic;
using System.Text;
using MassTransit.Context;
using MassTransit.Transports;
using Newtonsoft.Json;

namespace MassTransit.EventStoreDbIntegration.Serializers
{
    public static class DictionaryHeadersSerialize
    {
        static readonly Encoding _encoding;
        static readonly Lazy<IHeadersDeserializer> _deserializer;
        static readonly Lazy<IHeadersSerializer> _serializer;

        static DictionaryHeadersSerialize()
        {
            _encoding = new UTF8Encoding(false);
            _deserializer = new Lazy<IHeadersDeserializer>(() => new DictionaryHeadersDeserializer());
            _serializer = new Lazy<IHeadersSerializer>(() => new DictionaryHeadersSerializer());
        }

        public static IHeadersDeserializer Deserializer => _deserializer.Value;
        public static IHeadersSerializer Serializer => _serializer.Value;


        class DictionaryHeadersDeserializer :
            IHeadersDeserializer
        {
            public IHeaderProvider Deserialize(byte[] headers)
            {
                var jsonData = _encoding.GetString(headers);
                var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

                return new DictionaryHeaderProvider(dictionary);
            }
        }


        class DictionaryHeadersSerializer :
            IHeadersSerializer
        {
            const string CorrelationIdKey = "$correlationId";
            const string CausationIdKey = "$causationId";
            const string InstanceIdKey = "InstanceId";

            public byte[] Serialize(SendContext context)
            {
                var dictionary = new Dictionary<string, object>();

                if (context.RequestId.HasValue)
                    dictionary.Add(nameof(MessageContext.RequestId), context.RequestId.Value);

                if (context.CorrelationId.HasValue)
                    dictionary.Add(InstanceIdKey, context.CorrelationId.Value);

                if (context.ConversationId.HasValue)
                    dictionary.Add(CorrelationIdKey, context.ConversationId.Value);

                if (context.InitiatorId.HasValue)
                    dictionary.Add(CausationIdKey, context.InitiatorId.Value);

                if (context.SourceAddress != null)
                    dictionary.Add(nameof(MessageContext.SourceAddress), context.SourceAddress.ToString());

                if (context.DestinationAddress != null)
                    dictionary.Add(nameof(MessageContext.DestinationAddress), context.DestinationAddress.ToString());

                foreach (var headerValue in context.Headers)
                    dictionary.Add(headerValue.Key, headerValue.Value);

                return _encoding.GetBytes(JsonConvert.SerializeObject(dictionary));
            }
        }
    }
}
