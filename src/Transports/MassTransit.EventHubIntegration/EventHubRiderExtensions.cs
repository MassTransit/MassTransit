namespace MassTransit.EventHubIntegration
{
    using System.Collections.Generic;
    using Contexts;
    using MassTransit.Registration;
    using Pipeline.Observables;


    public static class EventHubRiderExtensions
    {
        public static void ConnectEventHub(this IBusInstance busInstance, RiderObservable observers, IEventHubProducerSharedContext sharedContext,
            IEnumerable<IEventHubReceiveEndpoint> endpoints)
        {
            var rider = new EventHubRider(endpoints, sharedContext, observers);
            busInstance.ConnectRider(rider);
        }
    }
}
