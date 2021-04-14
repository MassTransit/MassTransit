using MassTransit.EventStoreDbIntegration;
using MassTransit.EventStoreDbIntegration.Specifications;

namespace MassTransit
{
    public static class EventStoreDbCheckpointStoreConfigurationExtensions
    {
        /// <summary>
        /// Use EventStoreDB to store checkpoints for this catch-up subscription.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="checkpointStreamName">
        /// The stream name to use for storing checkpoints. This must be unique.
        /// </param>
        public static void UseEventStoreDBCheckpointStore(this IEventStoreDbCatchupSubscriptionConfigurator configurator,
            StreamName checkpointStreamName)
        {
            configurator.SetCheckpointStore((client) => new EventStoreDbCheckpointStore(client, checkpointStreamName));
        }
    }
}
