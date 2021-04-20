using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using GreenPipes;
using MassTransit.EventStoreDbIntegration.Serializers;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public interface SubscriptionContext :
        PipeContext,
        ISubscriptionLockContext
    {
        SubscriptionSettings SubscriptionSettings { get; }
        IHeadersDeserializer HeadersDeserializer { get; }
        ICheckpointStore CheckpointStore { get; }
        
        event Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> ProcessEvent;
        event Action<StreamSubscription, SubscriptionDroppedReason, Exception> ProcessSubscriptionDropped;

        Task SubscribeAsync(CancellationToken cancellationToken = default);
        Task CloseAsync(CancellationToken cancellationToken = default);
    }
}
