namespace MassTransit.Transports.InMemory.Topology.Topologies
{
    using MassTransit.Topology;


    public interface IInMemoryHostTopology :
        IHostTopology
    {
        new IInMemoryMessagePublishTopology<T> Publish<T>()
            where T : class;
    }
}
