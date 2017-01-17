namespace MassTransit.Transports.InMemory.Topology
{
    using MassTransit.Topology.Configuration;


    public interface IInMemoryConsumeTopologyConfigurator :
        IConsumeTopologyConfigurator,
        IInMemoryConsumeTopology
    {
    }
}