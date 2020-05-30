namespace MassTransit.EventHubIntegration
{
    using GreenPipes;
    using Processors;
    using Registration;
    using Riders;


    public static class EventHubRiderExtensions
    {
        public static ConnectHandle ConnectEventHub(this IBusInstance busInstance, RiderObservable observers, params IEventHubReceiveEndpoint[] endpoints)
        {
            var rider = new EventHubRider(endpoints, observers);
            return busInstance.ConnectRider(rider);
        }
    }
}
