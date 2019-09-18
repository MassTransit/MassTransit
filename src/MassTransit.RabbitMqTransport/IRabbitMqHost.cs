namespace MassTransit.RabbitMqTransport
{
    using GreenPipes;
    using Integration;
    using Topology;


    public interface IRabbitMqHost :
        IHost,
        IReceiveConnector<IRabbitMqReceiveEndpointConfigurator>
    {
        IConnectionContextSupervisor ConnectionContextSupervisor { get; }

        IRetryPolicy ConnectionRetryPolicy { get; }

        RabbitMqHostSettings Settings { get; }

        new IRabbitMqHostTopology Topology { get; }
    }
}
