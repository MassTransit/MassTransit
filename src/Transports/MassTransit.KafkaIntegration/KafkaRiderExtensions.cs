namespace MassTransit.KafkaIntegration
{
    using System.Collections.Generic;
    using GreenPipes;
    using MassTransit.Registration;
    using Pipeline.Observables;
    using Transport;


    public static class KafkaRiderExtensions
    {
        public static ConnectHandle ConnectKafka(this IBusInstance busInstance, RiderObservable observers,
            IEnumerable<IKafkaReceiveEndpoint> endpoints,
            IEnumerable<IKafkaProducerFactory> producers)
        {
            var rider = new KafkaRider(busInstance.HostConfiguration.HostAddress, endpoints, producers, observers);

            return busInstance.ConnectRider(rider);
        }
    }
}
