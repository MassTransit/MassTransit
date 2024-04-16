namespace MassTransit.KafkaIntegration.Serializers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Confluent.Kafka;
    using Transports;


    public static class DictionaryHeadersSerialize
    {
        static readonly Lazy<IHeadersDeserializer> _deserializer;
        static readonly Lazy<IHeadersSerializer> _serializer;

        static DictionaryHeadersSerialize()
        {
            _deserializer = new Lazy<IHeadersDeserializer>(() => new DictionaryHeadersDeserializer());
            _serializer = new Lazy<IHeadersSerializer>(() => new DictionaryHeadersSerializer(
                new TransportSetHeaderAdapter<Header>(new KafkaHeaderValueConverter(), TransportHeaderOptions.IncludeFaultMessage)));
        }

        public static IHeadersDeserializer Deserializer => _deserializer.Value;
        public static IHeadersSerializer Serializer => _serializer.Value;


        class DictionaryHeadersDeserializer :
            IHeadersDeserializer
        {
            public IHeaderProvider Deserialize(Headers headers)
            {
                var dictionary = headers.GroupBy(header => header.Key)
                                 .ToDictionary(group => group.Key, group =>
                                 {
                                     var valueBytes = group.First().GetValueBytes();
                                     return valueBytes != null ? (object)Encoding.UTF8.GetString(valueBytes) : null;
                                 });

                return new DictionaryHeaderProvider(dictionary);
            }
        }


        class DictionaryHeadersSerializer :
            IHeadersSerializer
        {
            readonly ITransportSetHeaderAdapter<Header> _adapter;

            public DictionaryHeadersSerializer(ITransportSetHeaderAdapter<Header> adapter)
            {
                _adapter = adapter;
            }

            public Headers Serialize(SendContext context)
            {
                var dictionary = new Dictionary<string, Header>();

                foreach (var headerValue in context.Headers)
                    _adapter.Set(dictionary, headerValue);

                if (context.MessageId.HasValue)
                    _adapter.Set(dictionary, nameof(MessageContext.MessageId), context.MessageId.Value);

                if (context.CorrelationId.HasValue)
                    _adapter.Set(dictionary, nameof(MessageContext.CorrelationId), context.CorrelationId.Value);

                if (context.ConversationId.HasValue)
                    _adapter.Set(dictionary, nameof(MessageContext.ConversationId), context.ConversationId.Value);

                if (context.InitiatorId.HasValue)
                    _adapter.Set(dictionary, nameof(MessageContext.InitiatorId), context.InitiatorId.Value);

                if (context.DestinationAddress != null)
                    _adapter.Set(dictionary, nameof(MessageContext.DestinationAddress), context.DestinationAddress.ToString());

                if (context.SourceAddress != null)
                    _adapter.Set(dictionary, nameof(MessageContext.SourceAddress), context.SourceAddress.ToString());

                if (context.ContentType != null)
                    _adapter.Set(dictionary, MessageHeaders.ContentType, context.ContentType.ToString());

                var headers = new Headers();
                foreach (var value in dictionary.Values)
                    headers.Add(value);

                return headers;
            }
        }
    }
}
