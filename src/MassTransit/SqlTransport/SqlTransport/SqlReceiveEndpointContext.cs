namespace MassTransit.SqlTransport
{
    using Topology;
    using Transports;


    public interface SqlReceiveEndpointContext :
        ReceiveEndpointContext
    {
        IClientContextSupervisor ClientContextSupervisor { get; }

        BrokerTopology BrokerTopology { get; }
    }
}
