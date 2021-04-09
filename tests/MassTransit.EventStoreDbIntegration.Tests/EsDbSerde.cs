using System;
using System.Collections.Generic;
using System.Text.Json;
using EventStore.Client;

namespace MassTransit.EventStoreDbIntegration.Tests
{
    public static class EsDbSerde
    {
        static readonly JsonSerializerOptions _options;
        static EsDbSerde()
        {
            _options = new JsonSerializerOptions();
        }

        public static object DeserializeEvent(this ResolvedEvent resolvedEvent) => resolvedEvent.Deserialize();

        public static EventData SerializeEvent(this object @event, Uuid eventId, Dictionary<string, object> eventMetadata)
        {
            Type eventType = @event.GetType();
            var eventTypeName = eventType.FullName;
            byte[] data = JsonSerializer.SerializeToUtf8Bytes(@event, eventType, _options);
            byte[] metadata = JsonSerializer.SerializeToUtf8Bytes(eventMetadata, eventMetadata.GetType(), _options);

            return new EventData(eventId, eventTypeName, data, metadata);
        }

        public static T DeserializeMetadata<T>(this ResolvedEvent resolvedEvent)
        {
            var utf8Reader = new Utf8JsonReader(resolvedEvent.Event.Metadata.ToArray());
            var metadata = JsonSerializer.Deserialize<T>(ref utf8Reader, _options);
            return metadata;
        }

        static object Deserialize(this ResolvedEvent resolvedEvent)
        {
            Type eventType = Type.GetType(resolvedEvent.Event.EventType);
            var utf8Reader = new Utf8JsonReader(resolvedEvent.Event.Data.ToArray());
            var @event = JsonSerializer.Deserialize(ref utf8Reader, eventType, _options);
            return @event;
        }

        static T Deserialize<T>(this ResolvedEvent resolvedEvent)
        {
            var utf8Reader = new Utf8JsonReader(resolvedEvent.Event.Data.ToArray());
            T @event = JsonSerializer.Deserialize<T>(ref utf8Reader, _options);
            return @event;
        }
    }
}
