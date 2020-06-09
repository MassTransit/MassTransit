namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Producer;
    using Context;
    using Pipeline;
    using Pipeline.Observables;


    public interface IEventHubProducerContext :
        ISendPipe
    {
        Uri HostAddress { get; }
        ILogContext LogContext { get; }
        SendObservable SendObservers { get; }
        IMessageSerializer Serializer { get; }

        Task Produce(EventDataBatch eventDataBatch, CancellationToken cancellationToken);
        Task Produce(IEnumerable<EventData> eventData, SendEventOptions options, CancellationToken cancellationToken);
        ValueTask<EventDataBatch> CreateBatch(CreateBatchOptions options, CancellationToken cancellationToken);
    }
}
