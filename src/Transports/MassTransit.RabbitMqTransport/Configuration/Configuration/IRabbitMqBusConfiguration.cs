namespace MassTransit.RabbitMqTransport.Configuration
{
    using MassTransit.Configuration;


    public interface IRabbitMqBusConfiguration :
        IBusConfiguration
    {
        new IRabbitMqHostConfiguration HostConfiguration { get; }

        new IRabbitMqEndpointConfiguration BusEndpointConfiguration { get; }

        new IRabbitMqTopologyConfiguration Topology { get; }

        /// <summary>
        /// Create an endpoint configuration on the bus, which can later be turned into a receive endpoint
        /// </summary>
        /// <returns></returns>
        IRabbitMqEndpointConfiguration CreateEndpointConfiguration();
    }
}
