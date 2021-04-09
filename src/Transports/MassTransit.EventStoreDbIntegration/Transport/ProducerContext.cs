using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using GreenPipes;

namespace MassTransit.EventStoreDbIntegration
{
    public interface ProducerContext :
        PipeContext,
        IAsyncDisposable
    {
        IMessageSerializer Serializer { get; }

        Task Produce(StreamName streamName, IEnumerable<EventData> eventData, CancellationToken cancellationToken);
        Task Produce(StreamName streamName, long version, IEnumerable<EventData> eventData, CancellationToken cancellationToken);
    }
}
