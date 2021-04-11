using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using GreenPipes;
using MassTransit.EventStoreDbIntegration.Serializers;

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
        public IHeadersSerializer HeadersSerializer { get; }

        public Task Produce(string streamName, IEnumerable<EventData> eventData, CancellationToken cancellationToken)
        {
            return _producerClient.AppendToStreamAsync(streamName, StreamState.Any, eventData, null, null, cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask(Task.CompletedTask);
        }
    }
}
