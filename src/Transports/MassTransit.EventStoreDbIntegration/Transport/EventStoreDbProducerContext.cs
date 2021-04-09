using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using GreenPipes;

namespace MassTransit.EventStoreDbIntegration
{
    public class EventStoreDbProducerContext :
        BasePipeContext,
        ProducerContext
    {
        readonly EventStoreClient _producerClient;

        public EventStoreDbProducerContext(EventStoreClient producerClient, IMessageSerializer messageSerializer,
            CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _producerClient = producerClient;
            Serializer = messageSerializer;
        }

        public IMessageSerializer Serializer { get; }

        public Task Produce(StreamName streamName, IEnumerable<EventData> eventData, CancellationToken cancellationToken)
        {
            return _producerClient.AppendToStreamAsync(streamName, StreamState.Any, eventData, null, null, cancellationToken);
        }

        public Task Produce(StreamName streamName, long version, IEnumerable<EventData> eventData, CancellationToken cancellationToken)
        {
            return version == -1
                ? _producerClient.AppendToStreamAsync(streamName, StreamState.NoStream, eventData, null, null, cancellationToken)
                : _producerClient.AppendToStreamAsync(streamName, Convert.ToUInt64(version), eventData, null, null, cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask(Task.CompletedTask);
        }
    }
}
