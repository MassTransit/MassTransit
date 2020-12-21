namespace MassTransit.EventHubIntegration
{
    using System.Collections.Generic;
    using Contexts;
    using MassTransit.Registration;
    using Pipeline.Observables;


    public static class EventHubRiderExtensions
    {
        public static void ConnectEventHub(this IBusInstance busInstance, RiderObservable observers, IEventHubProducerSharedContext sharedContext,
            IDictionary<string, IReceiveEndpointControl> endpoints)
        {
            var rider = new EventHubRider(endpoints, sharedContext);
            busInstance.ConnectRider(rider);
        }
    }
}
