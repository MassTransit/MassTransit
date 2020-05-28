namespace MassTransit.Transports.InMemory.Topology.Configurators
{
    using MassTransit.Topology;


    public interface IInMemoryPublishTopologyConfigurator :
        IPublishTopologyConfigurator,
        IInMemoryPublishTopology
    {
        new IInMemoryMessagePublishTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;
    }
}
