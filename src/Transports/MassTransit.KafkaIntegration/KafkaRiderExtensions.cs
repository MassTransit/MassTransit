namespace MassTransit.KafkaIntegration
{
    using Registration;
    using Riders;
    using Subscriptions;


    public static class KafkaRiderExtensions
    {
        public static void AddKafka(this IBusInstance busInstance, RiderObservable observers, params IKafkaReceiveEndpoint[] endpoints)
        {
            var rider = new KafkaRider(endpoints, observers);
            busInstance.Add(rider);
        }
    }
}
