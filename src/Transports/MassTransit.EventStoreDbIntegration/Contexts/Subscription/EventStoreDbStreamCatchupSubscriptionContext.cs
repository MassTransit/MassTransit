using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using MassTransit.Configuration;
using MassTransit.EventStoreDbIntegration.Serializers;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class EventStoreDbStreamCatchupSubscriptionContext :
        EventStoreDbCatchupSubscriptionContext
    {
        public EventStoreDbStreamCatchupSubscriptionContext(IHostConfiguration hostConfiguration, SubscriptionSettings subscriptionSettings, EventStoreClient client,
            IHeadersDeserializer headersDeserializer, ICheckpointStore checkpointStore, CancellationToken cancellationToken)
            : base(hostConfiguration, subscriptionSettings, client, headersDeserializer, checkpointStore, cancellationToken)
        {
        }

        public override async Task SubscribeAsync(CancellationToken cancellationToken = default)
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

        public override Task CheckpointReached(StreamSubscription streamSubscription, Position position, CancellationToken cancellationToken)
        {
            throw new NotSupportedException("CheckpointReached is only supported for All Stream subscriptions.");
        }
    }
}
