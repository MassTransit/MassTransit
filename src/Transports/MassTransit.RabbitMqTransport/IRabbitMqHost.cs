namespace MassTransit.RabbitMqTransport
{
    using Topology;


    public interface IRabbitMqHost :
        IHost,
        IReceiveConnector<IRabbitMqReceiveEndpointConfigurator>
    {
        new IRabbitMqHostTopology Topology { get; }
    }
}
