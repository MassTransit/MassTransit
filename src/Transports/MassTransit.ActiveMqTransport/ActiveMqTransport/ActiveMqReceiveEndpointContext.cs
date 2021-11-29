namespace MassTransit.ActiveMqTransport
{
    using Topology;
    using Transports;


    public interface ActiveMqReceiveEndpointContext :
        ReceiveEndpointContext
    {
        BrokerTopology BrokerTopology { get; }

        ISessionContextSupervisor SessionContextSupervisor { get; }
    }
}
