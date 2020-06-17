namespace MassTransit.EventHubIntegration
{
    using System.Collections.Generic;
    using Contexts;
    using GreenPipes;
    using MassTransit.Registration;
    using Pipeline.Observables;


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
