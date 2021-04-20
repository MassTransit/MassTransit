using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;
using EventStore.Client;
using MassTransit.Context;
using MassTransit.Serialization;
using MassTransit.Transports;
using Newtonsoft.Json;

namespace MassTransit.EventStoreDbIntegration.Serializers
{
    public static class DictionaryHeadersSerde
    {
        const string EsDbCorrelationIdKey = "$correlationId";
        const string EsDbCausationIdKey = "$causationId";
        const string InstanceIdKey = "InstanceId";
        const string ClrTypeKey = "ClrType";

        static readonly Encoding _encoding;
        static readonly Lazy<IHeadersDeserializer> _deserializer;
        static readonly Lazy<IHeadersSerializer> _serializer;

        static DictionaryHeadersSerde()
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
            public IHeaderProvider Deserialize(ResolvedEvent resolvedEvent)
            {
                var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(_encoding.GetString(resolvedEvent.Event.Metadata.ToArray()));

                if (dictionary.ContainsKey(EsDbCorrelationIdKey))
                {
                    dictionary.Add(MessageHeaders.ConversationId, dictionary[EsDbCorrelationIdKey]);
                    dictionary.Remove(EsDbCorrelationIdKey);
                }

                if (dictionary.ContainsKey(EsDbCausationIdKey))
                {
                    dictionary.Add(MessageHeaders.InitiatorId, dictionary[EsDbCausationIdKey]);
                    dictionary.Remove(EsDbCausationIdKey);
                }

                if (!dictionary.ContainsKey(MessageHeaders.ContentType) && resolvedEvent.Event.ContentType == MediaTypeNames.Application.Json)
                {
                    dictionary.Add(MessageHeaders.ContentType, MediaTypeNames.Application.Json);
                }

                if (dictionary.ContainsKey(InstanceIdKey))
                {
                    dictionary.Add(MessageHeaders.CorrelationId, dictionary[InstanceIdKey]);
                    dictionary.Remove(InstanceIdKey);
                }

                return new DictionaryHeaderProvider(dictionary);
            }
        }


        class DictionaryHeadersSerializer :
            IHeadersSerializer
        {
            public byte[] Serialize<T>(SendContext<T> context)
                where T : class
            {
                var dictionary = new Dictionary<string, object>();

                if (context.ConversationId.HasValue)
                    dictionary.Add(EsDbCorrelationIdKey, context.ConversationId.Value);

                if (context.InitiatorId.HasValue)
                    dictionary.Add(EsDbCausationIdKey, context.InitiatorId.Value);

                dictionary.Add(MessageHeaders.ContentType, context.ContentType?.MediaType ?? JsonMessageSerializer.ContentTypeHeaderValue);

                dictionary.Add(ClrTypeKey, typeof(T).FullName);

                //If UseRawJsonSerializer
                if (context.ContentType.MediaType.Equals(MediaTypeNames.Application.Json))
                {
                    foreach (var headerValue in context.Headers)
                    {
                        switch (headerValue.Key)
                        {
                            case MessageHeaders.ConversationId:
                            case MessageHeaders.InitiatorId:
                            case MessageHeaders.ContentType:
                                //Do nothing, already added above
                                break;

                            case MessageHeaders.CorrelationId:
                                dictionary.Add(InstanceIdKey, headerValue.Value);
                                break;

                            default:
                                dictionary.Add(headerValue.Key, headerValue.Value);
                                break;
                        }
                    }
                }

                return _encoding.GetBytes(JsonConvert.SerializeObject(dictionary));
            }
        }
    }
}
