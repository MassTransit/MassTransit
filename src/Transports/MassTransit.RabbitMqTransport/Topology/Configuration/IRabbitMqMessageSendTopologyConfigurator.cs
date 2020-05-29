namespace MassTransit.RabbitMqTransport.Topology
{
    using MassTransit.Topology;


    public interface IRabbitMqMessageSendTopologyConfigurator<TMessage> :
        IMessageSendTopologyConfigurator<TMessage>,
        IRabbitMqMessageSendTopology<TMessage>,
        IRabbitMqMessageSendTopologyConfigurator
        where TMessage : class
    {
    }


    public interface IRabbitMqMessageSendTopologyConfigurator :
        IMessageSendTopologyConfigurator
    {
    }
}
