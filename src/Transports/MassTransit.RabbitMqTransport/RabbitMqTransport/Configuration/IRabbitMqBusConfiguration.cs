namespace MassTransit.RabbitMqTransport.Configuration
{
    using MassTransit.Configuration;


    public interface IRabbitMqBusConfiguration :
        IBusConfiguration
    {
        new IRabbitMqHostConfiguration HostConfiguration { get; }

        new IRabbitMqEndpointConfiguration BusEndpointConfiguration { get; }

        new IRabbitMqTopologyConfiguration Topology { get; }

        IRabbitMqEndpointConfiguration CreateEndpointConfiguration(bool isBusEndpoint = false);
    }
}
