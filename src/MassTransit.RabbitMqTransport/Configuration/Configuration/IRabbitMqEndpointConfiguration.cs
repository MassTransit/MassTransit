namespace MassTransit.RabbitMqTransport.Configuration
{
    using MassTransit.Configuration;


    public interface IRabbitMqEndpointConfiguration :
        IEndpointConfiguration
    {
        new IRabbitMqTopologyConfiguration Topology { get; }
    }
}
