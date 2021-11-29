namespace MassTransit
{
    using RabbitMqTransport.Topology;


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
