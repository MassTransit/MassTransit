namespace MassTransit.ActiveMqTransport.Topology.Topologies
{
    using MassTransit.Topology.Topologies;


    public class ActiveMqMessageSendTopology<TMessage> :
        MessageSendTopology<TMessage>,
        IActiveMqMessageSendTopologyConfigurator<TMessage>
        where TMessage : class
    {
    }
}
