namespace MassTransit.KafkaIntegration
{
    using System.Collections.Generic;
    using GreenPipes;
    using MassTransit.Registration;
    using Transport;


    public static class KafkaRiderExtensions
    {
        public static void ConnectKafka(this IBusInstance busInstance,
            IDictionary<string, IReceiveEndpointControl> endpoints,
            IEnumerable<IKafkaProducerFactory> producers)
        {
            var rider = new KafkaRider(busInstance.HostConfiguration.HostAddress, endpoints, producers);

            busInstance.ConnectRider(rider);
        }
    }
}
