namespace MassTransit.ActiveMqTransport
{
    using GreenPipes;
    using Topology;
    using Transport;


    public interface IActiveMqHost :
        IHost,
        IReceiveConnector<IActiveMqReceiveEndpointConfigurator>
    {
        IConnectionContextSupervisor ConnectionContextSupervisor { get; }

        IRetryPolicy ConnectionRetryPolicy { get; }

        ActiveMqHostSettings Settings { get; }

        new IActiveMqHostTopology Topology { get; }
    }
}
