namespace MassTransit.Transports.InMemory.Topology.Configurators
{
    using MassTransit.Topology;


    public interface IInMemoryConsumeTopologyConfigurator :
        IConsumeTopologyConfigurator,
        IInMemoryConsumeTopology
    {
        new IInMemoryMessageConsumeTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;

        void AddSpecification(IInMemoryConsumeTopologySpecification specification);
    }
}
