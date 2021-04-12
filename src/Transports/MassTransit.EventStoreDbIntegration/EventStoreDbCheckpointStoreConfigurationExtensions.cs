using System;
using MassTransit.Serialization;
using Newtonsoft.Json;

namespace MassTransit.EventStoreDbIntegration
{
    public static class EventStoreDbCheckpointStoreConfigurationExtensions
    {
        /// <summary>
        /// Serialize messages using the JSON serializer
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseEventStoreDBCheckpointStore(this IEventStoreDbCatchupSubscriptionConfigurator configurator)
        {
            //configurator.SetCheckpointStore(() => new EventStoreDbCheckpointStore(configurator.));
        }
    }
}
