namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Producer;


    public interface ProducerContext :
        PipeContext,
        IAsyncDisposable
    {
        ISerialization Serializer { get; }

        Task Produce(EventDataBatch eventDataBatch, CancellationToken cancellationToken);
        Task Produce(IEnumerable<EventData> eventData, SendEventOptions options, CancellationToken cancellationToken);
        ValueTask<EventDataBatch> CreateBatch(CreateBatchOptions options, CancellationToken cancellationToken);
    }
}
