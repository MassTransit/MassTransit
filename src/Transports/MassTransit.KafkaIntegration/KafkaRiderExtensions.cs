namespace MassTransit
{
    using GreenPipes;
    using KafkaIntegration.Transport;
    using Registration;
    using Riders;


    public static class KafkaRiderExtensions
    {
        public static ConnectHandle ConnectKafka(this IBusInstance busInstance, RiderObservable observers, params IKafkaReceiveEndpoint[] endpoints)
        {
            var rider = new KafkaRider(endpoints, observers);
            return busInstance.ConnectRider(rider);
        }
    }
}
