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
        const string CorrelationIdKey = "$correlationId";
        const string CausationIdKey = "$causationId";
        const string InstanceIdKey = "InstanceId";
        const string ContentTypeKey = "Content-Type";
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

                if (dictionary.ContainsKey(CorrelationIdKey))
                {
                    dictionary.Add(nameof(MessageContext.ConversationId), dictionary[CorrelationIdKey]);
                    dictionary.Remove(CorrelationIdKey);
                }

                if (dictionary.ContainsKey(CausationIdKey))
                {
                    dictionary.Add(nameof(MessageContext.InitiatorId), dictionary[CausationIdKey]);
                    dictionary.Remove(CausationIdKey);
                }

                if (!dictionary.ContainsKey(ContentTypeKey) && resolvedEvent.Event.ContentType == MediaTypeNames.Application.Json)
                {
                    dictionary.Add(ContentTypeKey, MediaTypeNames.Application.Json);
                }

                if (dictionary.ContainsKey(InstanceIdKey))
                {
                    dictionary.Add(nameof(MessageContext.CorrelationId), dictionary[InstanceIdKey]);
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
                    dictionary.Add(CorrelationIdKey, context.ConversationId.Value);

                if (context.InitiatorId.HasValue)
                    dictionary.Add(CausationIdKey, context.InitiatorId.Value);

                dictionary.Add(ContentTypeKey, context.ContentType?.MediaType ?? JsonMessageSerializer.ContentTypeHeaderValue);

                dictionary.Add(ClrTypeKey, typeof(T).FullName);

                //If UseRawJsonSerializer
                if (context.ContentType.MediaType.Equals(MediaTypeNames.Application.Json))
                {
                    if (context.RequestId.HasValue)
                        dictionary.Add(nameof(MessageContext.RequestId), context.RequestId.Value);

                    if (context.CorrelationId.HasValue)
                        dictionary.Add(InstanceIdKey, context.CorrelationId.Value);

                    if (context.ScheduledMessageId.HasValue)
                        dictionary.Add(nameof(context.ScheduledMessageId), context.ScheduledMessageId.Value);

                    if (context.SourceAddress != null)
                        dictionary.Add(nameof(MessageContext.SourceAddress), context.SourceAddress.ToString());

                    if (context.DestinationAddress != null)
                        dictionary.Add(nameof(MessageContext.DestinationAddress), context.DestinationAddress.ToString());

                    if (context.ResponseAddress != null)
                        dictionary.Add(nameof(MessageContext.ResponseAddress), context.ResponseAddress.ToString());

                    if (context.FaultAddress != null)
                        dictionary.Add(nameof(MessageContext.FaultAddress), context.FaultAddress.ToString());

                    if (context.TimeToLive.HasValue)
                        dictionary.Add(nameof(MessageContext.ExpirationTime), DateTime.UtcNow.Add(context.TimeToLive.Value));

                    if (context.SentTime.HasValue)
                        dictionary.Add(nameof(MessageContext.SentTime), context.SentTime.Value);

                    foreach (var headerValue in context.Headers)
                        dictionary.Add(headerValue.Key, headerValue.Value);
                }

                return _encoding.GetBytes(JsonConvert.SerializeObject(dictionary));
            }
        }
    }
}
