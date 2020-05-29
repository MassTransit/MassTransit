namespace MassTransit.KafkaIntegration
{
    using GreenPipes;
    using Registration;
    using Riders;
    using Subscriptions;


    public static class KafkaRiderExtensions
    {
        public static ConnectHandle ConnectKafka(this IBusInstance busInstance, RiderObservable observers, params IKafkaReceiveEndpoint[] endpoints)
        {
            var rider = new KafkaRider(endpoints, observers);
            return busInstance.ConnectRider(rider);
        }
    }
}
