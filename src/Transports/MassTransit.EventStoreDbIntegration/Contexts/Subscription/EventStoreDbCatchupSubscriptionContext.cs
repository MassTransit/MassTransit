using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using GreenPipes;
using MassTransit.Configuration;
using MassTransit.EventStoreDbIntegration.Serializers;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class EventStoreDbCatchupSubscriptionContext :
        BasePipeContext,
        SubscriptionContext
    {
        readonly EventStoreClient _client;
        readonly ISubscriptionLockContext _lockContext;

        StreamSubscription _streamSubscription;

        public EventStoreDbCatchupSubscriptionContext(IHostConfiguration hostConfiguration, SubscriptionSettings receiveSettings, EventStoreClient client,
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
 
            _streamSubscription = position.HasValue
                ? await _client.SubscribeToStreamAsync(
                    streamName: SubscriptionSettings.StreamName,
                    start: StreamPosition.FromInt64((long)position),
                    eventAppeared: EventAppeared,
                    resolveLinkTos: true,
                    subscriptionDropped: SubscriptionDropped,
                    configureOperationOptions: null,
                    userCredentials: SubscriptionSettings.UserCredentials,
                    cancellationToken: cancellationToken)
                : await _client.SubscribeToStreamAsync(
                    streamName: SubscriptionSettings.StreamName,
                    eventAppeared: EventAppeared,
                    resolveLinkTos: true,
                    subscriptionDropped: SubscriptionDropped,
                    configureOperationOptions: null,
                    userCredentials: SubscriptionSettings.UserCredentials,
                    cancellationToken: cancellationToken);
        }

        public Task CloseAsync(CancellationToken cancellationToken = default)
        {
            _streamSubscription.Dispose();
            return Task.CompletedTask;
        }

        public Task Complete(ResolvedEvent resolvedEvent) => _lockContext.Complete(resolvedEvent);

        public Task CheckpointReached(StreamSubscription streamSubscription, Position position, CancellationToken cancellationToken)
        {
            throw new NotSupportedException("CheckpointReached is only supported for All Stream subscriptions.");
        }

        async Task EventAppeared(StreamSubscription streamSubscription, ResolvedEvent resolvedEvent, CancellationToken cancellation)
        {
            if (ProcessEvent != null)
                await ProcessEvent.Invoke(streamSubscription, resolvedEvent, cancellation);
        }

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        void SubscriptionDropped(StreamSubscription streamSubscription, SubscriptionDroppedReason reason, Exception? exc)
        {
            //TODO: Need to implement reconnect manually or handled by MT?
            if (ProcessSubscriptionDropped != null)
                ProcessSubscriptionDropped.Invoke(streamSubscription, reason, exc);
        }
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    }
}
