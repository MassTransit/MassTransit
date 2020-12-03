namespace MassTransit.ActiveMqTransport.Contexts
{
    using Context;
    using Topology.Builders;
    using Transport;


    public interface ActiveMqReceiveEndpointContext :
        ReceiveEndpointContext
    {
        BrokerTopology BrokerTopology { get; }

        ISessionContextSupervisor SessionContextSupervisor { get; }
    }
}
