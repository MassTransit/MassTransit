namespace MassTransit.RabbitMqTransport.Topology.Topologies
{
    using MassTransit.Topology.Topologies;


    public class RabbitMqMessageSendTopology<TMessage> :
        MessageSendTopology<TMessage>,
        IRabbitMqMessageSendTopologyConfigurator<TMessage>
        where TMessage : class
    {
    }
}
