namespace MassTransit.SqlTransport.Topology
{
    using MassTransit.Topology;


    public class SqlMessageSendTopology<TMessage> :
        MessageSendTopology<TMessage>,
        ISqlMessageSendTopologyConfigurator<TMessage>
        where TMessage : class
    {
    }
}
