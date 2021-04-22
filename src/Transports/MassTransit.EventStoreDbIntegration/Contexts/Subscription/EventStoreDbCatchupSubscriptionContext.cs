using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using GreenPipes;
using MassTransit.Configuration;
using MassTransit.EventStoreDbIntegration.Serializers;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public abstract class EventStoreDbCatchupSubscriptionContext :
        BasePipeContext,
        SubscriptionContext
    {
        protected readonly EventStoreClient _client;
        protected readonly ISubscriptionLockContext _lockContext;

        protected StreamSubscription _streamSubscription;

        public EventStoreDbCatchupSubscriptionContext(IHostConfiguration hostConfiguration, SubscriptionSettings subscriptionSettings, EventStoreClient client,
            IHeadersDeserializer headersDeserializer, ICheckpointStore checkpointStore, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _client = client;

            _lockContext = new EventStoreDbCatchupSubscriptionLockContext(hostConfiguration, subscriptionSettings, checkpointStore);

            SubscriptionSettings = subscriptionSettings;
            HeadersDeserializer = headersDeserializer;
            CheckpointStore = checkpointStore;
        }

        public event Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> ProcessEvent;
        public event Action<StreamSubscription, SubscriptionDroppedReason, Exception> ProcessSubscriptionDropped;

        public SubscriptionSettings SubscriptionSettings { get; }
        public IHeadersDeserializer HeadersDeserializer { get; }
        public ICheckpointStore CheckpointStore { get; }

        public abstract Task SubscribeAsync(CancellationToken cancellationToken = default);

        public Task CloseAsync(CancellationToken cancellationToken = default)
        {
            _streamSubscription.Dispose();
            return Task.CompletedTask;
        }

        public Task Complete(ResolvedEvent resolvedEvent) => _lockContext.Complete(resolvedEvent);

        public abstract Task CheckpointReached(StreamSubscription streamSubscription, Position position, CancellationToken cancellationToken);

        protected async Task EventAppeared(StreamSubscription streamSubscription, ResolvedEvent resolvedEvent, CancellationToken cancellation)
        {
            if (ProcessEvent != null)
                await ProcessEvent.Invoke(streamSubscription, resolvedEvent, cancellation);
        }

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        protected void SubscriptionDropped(StreamSubscription streamSubscription, SubscriptionDroppedReason reason, Exception? exc)
        {
            ProcessSubscriptionDropped?.Invoke(streamSubscription, reason, exc);
        }
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    }
}
