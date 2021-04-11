using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using GreenPipes;
using MassTransit.EventStoreDbIntegration.Serializers;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public interface ProcessorContext :
        PipeContext,
        IProcessorLockContext
    {
        ReceiveSettings ReceiveSettings { get; }
        ICheckpointStore CheckpointStore { get; }
        IHeadersDeserializer MetadataDeserializer { get; }

        event Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> ProcessEvent;
        event Action<StreamSubscription, SubscriptionDroppedReason, Exception> ProcessSubscriptionDropped;

        Task StartProcessingAsync(CancellationToken cancellationToken = default);
        Task StopProcessingAsync(CancellationToken cancellationToken = default);
    }
}
