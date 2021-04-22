using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using MassTransit.Configuration;
using MassTransit.EventStoreDbIntegration.Serializers;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class EventStoreDbAllStreamCatchupSubscriptionContext :
        EventStoreDbCatchupSubscriptionContext
    {
        public EventStoreDbAllStreamCatchupSubscriptionContext(IHostConfiguration hostConfiguration, SubscriptionSettings subscriptionSettings, EventStoreClient client,
            IHeadersDeserializer headersDeserializer, ICheckpointStore checkpointStore, CancellationToken cancellationToken)
            : base(hostConfiguration, subscriptionSettings, client, headersDeserializer, checkpointStore, cancellationToken)
        {
        }

        public override async Task SubscribeAsync(CancellationToken cancellationToken = default)
        {
            var position = await CheckpointStore.GetLastCheckpoint().ConfigureAwait(false);
 
            _streamSubscription = position.HasValue
                ? await _client.SubscribeToAllAsync(
                    new Position(position.Value, position.Value),
                    EventAppeared,
                    resolveLinkTos: false,
                    subscriptionDropped: SubscriptionDropped,
                    filterOptions: new SubscriptionFilterOptions(
                        SubscriptionSettings.AllStreamEventFilter ?? EventTypeFilter.ExcludeSystemEvents(),
                        SubscriptionSettings.CheckpointMessageCount,
                        CheckpointReached
                    ),
                    configureOperationOptions: null,
                    userCredentials: SubscriptionSettings.UserCredentials,
                    cancellationToken: cancellationToken)
                : await _client.SubscribeToAllAsync(
                    EventAppeared,
                    resolveLinkTos: false,
                    subscriptionDropped: SubscriptionDropped,
                    filterOptions: new SubscriptionFilterOptions(
                        SubscriptionSettings.AllStreamEventFilter ?? EventTypeFilter.ExcludeSystemEvents(),
                        SubscriptionSettings.CheckpointMessageCount,
                        CheckpointReached
                    ),
                    configureOperationOptions: null,
                    userCredentials: SubscriptionSettings.UserCredentials,
                    cancellationToken: cancellationToken);
        }

        public override async Task CheckpointReached(StreamSubscription streamSubscription, Position position, CancellationToken cancellationToken)
        {
            await _lockContext.CheckpointReached(streamSubscription, position, cancellationToken).ConfigureAwait(false);
        }
    }
}
