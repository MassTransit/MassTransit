namespace MassTransit.RabbitMqTransport.Topology
{
    using MassTransit.Topology;


    public class RabbitMqMessageSendTopology<TMessage> :
        MessageSendTopology<TMessage>,
        IRabbitMqMessageSendTopologyConfigurator<TMessage>
        where TMessage : class
    {
    }
}
