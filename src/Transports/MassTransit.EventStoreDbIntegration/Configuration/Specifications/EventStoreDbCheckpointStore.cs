using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EventStore.Client;

namespace MassTransit.EventStoreDbIntegration.Specifications
{
    public sealed class EventStoreDbCheckpointStore : ICheckpointStore
    {
        readonly EventStoreClient _client;
        readonly StreamName _streamName;

        public EventStoreDbCheckpointStore(EventStoreClient client, StreamName streamName)
        {
            _client = client;
            _streamName = streamName;
        }

        public async Task<ulong?> GetCheckpoint()
        {
            var result = _client.ReadStreamAsync(Direction.Backwards, _streamName, StreamPosition.End, 1);

            if (await result.ReadState.ConfigureAwait(false) == ReadState.StreamNotFound)
                return null;

            var eventData = await result.FirstAsync().ConfigureAwait(false);

            if (eventData.Equals(default(ResolvedEvent)))
            {
                await StoreCheckpoint(Position.Start.CommitPosition);
                return null;
            }

            return DeserializeCheckpoint()?.Position;

            Checkpoint DeserializeCheckpoint()
            {
                var utf8Reader = new Utf8JsonReader(eventData.Event.Data.ToArray());
                var @event = JsonSerializer.Deserialize<Checkpoint>(ref utf8Reader);
                return @event;
            }
        }

        public Task StoreCheckpoint(ulong? checkpoint)
        {
            var @event = new Checkpoint { Position = checkpoint };
            var preparedEvent = new EventData(Uuid.NewUuid(), "$checkpoint", JsonSerializer.SerializeToUtf8Bytes(@event));

            return _client.AppendToStreamAsync(_streamName, StreamState.Any, new List<EventData> { preparedEvent });
        }


        sealed class Checkpoint
        {
            public ulong? Position { get; set; }
        }
    }
}
