namespace MassTransit.InMemoryTransport.Configuration
{
    using MassTransit.Configuration;


    public interface IInMemoryConsumeTopologySpecification :
        ISpecification
    {
        void Apply(IMessageFabricConsumeTopologyBuilder builder);
    }
}
