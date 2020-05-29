namespace MassTransit.Transports.InMemory.Topology
{
    using MassTransit.Topology;


    public interface IInMemoryMessageConsumeTopology<TMessage> :
        IMessageConsumeTopology<TMessage>
        where TMessage : class
    {
    }
}
