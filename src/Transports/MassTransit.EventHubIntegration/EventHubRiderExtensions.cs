namespace MassTransit
{
    using System.Collections.Generic;
    using EventHubIntegration;
    using EventHubIntegration.Contexts;
    using GreenPipes;
    using Pipeline.Observables;
    using Registration;


    public static class EventHubRiderExtensions
    {
        public static ConnectHandle ConnectEventHub(this IBusInstance busInstance, RiderObservable observers, IEventHubProducerSharedContext sharedContext,
            IEnumerable<IEventHubReceiveEndpoint> endpoints)
        {
            var rider = new EventHubRider(endpoints, sharedContext, observers);
            return busInstance.ConnectRider(rider);
        }
    }
}
