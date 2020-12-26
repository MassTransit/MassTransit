namespace MassTransit.RabbitMqTransport
{
    using Topology;


    public interface IRabbitMqHost :
        IHost<IRabbitMqReceiveEndpointConfigurator>
    {
        new IRabbitMqHostTopology Topology { get; }
    }
}
