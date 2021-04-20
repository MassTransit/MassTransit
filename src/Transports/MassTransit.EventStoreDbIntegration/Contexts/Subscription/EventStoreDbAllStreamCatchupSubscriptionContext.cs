using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using GreenPipes;
using MassTransit.Configuration;
using MassTransit.EventStoreDbIntegration.Serializers;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class EventStoreDbAllStreamCatchupSubscriptionContext :
        BasePipeContext,
        SubscriptionContext
    {
        readonly EventStoreClient _client;
        readonly ISubscriptionLockContext _lockContext;

        StreamSubscription _streamSubscription;

        public EventStoreDbAllStreamCatchupSubscriptionContext(IHostConfiguration hostConfiguration, SubscriptionSettings receiveSettings, EventStoreClient client,
            IHeadersDeserializer headersDeserializer, ICheckpointStore checkpointStore, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _client = client;

            _lockContext = new EventStoreDbCatchupSubscriptionLockContext(hostConfiguration, receiveSettings, checkpointStore);

            SubscriptionSettings = receiveSettings;
            HeadersDeserializer = headersDeserializer;
            CheckpointStore = checkpointStore;
        }

        public event Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> ProcessEvent;
        public event Action<StreamSubscription, SubscriptionDroppedReason, Exception> ProcessSubscriptionDropped;

        public SubscriptionSettings SubscriptionSettings { get; }
        public IHeadersDeserializer HeadersDeserializer { get; }
        public ICheckpointStore CheckpointStore { get; }
        
        public async Task SubscribeAsync(CancellationToken cancellationToken = default)
        {
            var position = await CheckpointStore.GetLastCheckpoint().ConfigureAwait(false);
 
            _streamSubscription = await _client.SubscribeToAllAsync(
                    GetAllStreamPosition(),
                    EventAppeared,
                    resolveLinkTos: false,
                    subscriptionDropped: SubscriptionDropped,
                    filterOptions: null,
                    configureOperationOptions: null,
                    userCredentials: null,
                    cancellationToken: cancellationToken);

            Position GetAllStreamPosition() =>
                position.HasValue
                    ? new Position(position.Value, position.Value)
                    : Position.Start;
        }

        public Task CloseAsync(CancellationToken cancellationToken = default)
        {
            _streamSubscription.Dispose();
            return Task.CompletedTask;
        }

        public Task Complete(ResolvedEvent resolvedEvent) => _lockContext.Complete(resolvedEvent);

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        void SubscriptionDropped(StreamSubscription streamSubscription, SubscriptionDroppedReason reason, Exception? exc)
        {
            if (ProcessSubscriptionDropped != null)
                ProcessSubscriptionDropped.Invoke(streamSubscription, reason, exc);
        }
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

        async Task EventAppeared(StreamSubscription streamSubscription, ResolvedEvent resolvedEvent, CancellationToken cancellation)
        {
            if (resolvedEvent.Event.EventType.StartsWith("$"))
                return;

            if (ProcessEvent != null)
                await ProcessEvent.Invoke(streamSubscription, resolvedEvent, cancellation);
        }
    }
}
