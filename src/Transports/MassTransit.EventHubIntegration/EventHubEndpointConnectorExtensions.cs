namespace MassTransit.EventHubIntegration
{
    using System;
    using Azure.Messaging.EventHubs.Consumer;


    public static class EventHubEndpointConnectorExtensions
    {
        /// <summary>
        /// Connect to EventHub using default consumer group <see cref="EventHubConsumerClient.DefaultConsumerGroupName" />
        /// </summary>
        /// <param name="connector"></param>
        /// <param name="eventHubName">Event Hub name</param>
        /// <param name="configure"></param>
        public static HostReceiveEndpointHandle ConnectEventHubEndpoint(this IEvenHubEndpointConnector connector, string eventHubName,
            Action<IEventHubReceiveEndpointConfigurator> configure)
        {
            return connector.ConnectEventHubEndpoint(eventHubName, EventHubConsumerClient.DefaultConsumerGroupName, configure);
        }
    }
}
