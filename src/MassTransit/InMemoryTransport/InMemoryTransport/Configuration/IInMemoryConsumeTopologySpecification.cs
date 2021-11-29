namespace MassTransit.InMemoryTransport.Configuration
{
    public interface IInMemoryConsumeTopologySpecification :
        ISpecification
    {
        void Apply(IInMemoryConsumeTopologyBuilder builder);
    }
}
