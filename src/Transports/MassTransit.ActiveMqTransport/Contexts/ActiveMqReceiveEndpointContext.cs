namespace MassTransit.ActiveMqTransport.Contexts
{
    using Context;
    using Topology.Builders;


    public interface ActiveMqReceiveEndpointContext :
        ReceiveEndpointContext
    {
        BrokerTopology BrokerTopology { get; }
    }
}
