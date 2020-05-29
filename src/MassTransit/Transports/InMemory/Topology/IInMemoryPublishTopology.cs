namespace MassTransit.Transports.InMemory.Topology
{
    using MassTransit.Topology;


    public interface IInMemoryPublishTopology :
        IPublishTopology
    {
        new IInMemoryMessagePublishTopology<T> GetMessageTopology<T>()
            where T : class;
    }
}
