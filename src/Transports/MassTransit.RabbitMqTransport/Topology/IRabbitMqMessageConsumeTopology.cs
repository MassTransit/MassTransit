namespace MassTransit.RabbitMqTransport.Topology
{
    using MassTransit.Topology;


    public interface IRabbitMqMessageConsumeTopology<TMessage> :
        IMessageConsumeTopology<TMessage>
        where TMessage : class
    {
    }
}
