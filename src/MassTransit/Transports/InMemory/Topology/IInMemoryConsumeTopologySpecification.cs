namespace MassTransit.Transports.InMemory.Topology
{
    using GreenPipes;
    using InMemory.Builders;


    public interface IInMemoryConsumeTopologySpecification :
        ISpecification
    {
        void Apply(IInMemoryConsumeTopologyBuilder builder);
    }
}
