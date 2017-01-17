namespace MassTransit.Transports.InMemory.Topology
{
    using Builders;
    using GreenPipes;


    public interface IInMemoryConsumeTopologySpecification :
        ISpecification
    {
        void Apply(IInMemoryConsumeTopologyBuilder builder);
    }
}