namespace MassTransit.ActiveMqTransport.Topology
{
    using MassTransit.Topology;


    public class ActiveMqMessageSendTopology<TMessage> :
        MessageSendTopology<TMessage>,
        IActiveMqMessageSendTopologyConfigurator<TMessage>
        where TMessage : class
    {
    }
}
