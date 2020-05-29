namespace MassTransit.RabbitMqTransport.Topology
{
    using MassTransit.Topology;


    public interface IRabbitMqMessageSendTopology<TMessage> :
        IMessageSendTopology<TMessage>
        where TMessage : class
    {
    }
}
