using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using GreenPipes;
using MassTransit.EventStoreDbIntegration.Serializers;

namespace MassTransit.EventStoreDbIntegration
{
    public interface ProducerContext :
        PipeContext,
        IAsyncDisposable
    {
        IHeadersSerializer HeadersSerializer { get; }
        IMessageSerializer Serializer { get; }

        Task Produce(string streamName, IEnumerable<EventData> eventData, CancellationToken cancellationToken);
    }
}
