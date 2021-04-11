using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using GreenPipes;
using MassTransit.Configuration;
using MassTransit.EventStoreDbIntegration.Serializers;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class EventStoreDbProcessorContext :
        BasePipeContext,
        ProcessorContext
    {
        readonly EventStoreClient _client;
        readonly IProcessorLockContext _lockContext;

        StreamSubscription _streamSubscription;

        public EventStoreDbProcessorContext(IHostConfiguration hostConfiguration, ReceiveSettings receiveSettings, EventStoreClient client,
            ICheckpointStore checkpointStore, IHeadersDeserializer metadataDeserializer, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _client = client;

            _lockContext = new ProcessorLockContext(hostConfiguration, receiveSettings, checkpointStore);

            ReceiveSettings = receiveSettings;
            CheckpointStore = checkpointStore;
            MetadataDeserializer = metadataDeserializer;
        }

        public event Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> ProcessEvent;
        public event Action<StreamSubscription, SubscriptionDroppedReason, Exception> ProcessSubscriptionDropped;

        public ReceiveSettings ReceiveSettings { get; }
        public ICheckpointStore CheckpointStore { get; }
        public IHeadersDeserializer MetadataDeserializer { get; }

        public async Task StartProcessingAsync(CancellationToken cancellationToken = default)
        {
            var position = await CheckpointStore.GetCheckpoint().ConfigureAwait(false);

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

        public Task StopProcessingAsync(CancellationToken cancellationToken = default)
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
            if (ProcessEvent != null)
                await ProcessEvent.Invoke(streamSubscription, resolvedEvent, cancellation);
        }
    }
}
