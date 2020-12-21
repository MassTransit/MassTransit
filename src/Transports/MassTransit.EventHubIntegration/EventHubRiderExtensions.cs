namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Collections.Generic;
    using Contexts;
    using MassTransit.Registration;


    public static class EventHubRiderExtensions
    {
        public static void ConnectEventHub(this IBusInstance busInstance, Func<IEventHubProducerSharedContext> sharedContext,
            IDictionary<string, IReceiveEndpointControl> endpoints)
        {
            var rider = new EventHubRider(endpoints, sharedContext);
            busInstance.ConnectRider(rider);
        }
    }
}
