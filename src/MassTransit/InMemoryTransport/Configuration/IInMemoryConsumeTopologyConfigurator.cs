namespace MassTransit
{
    using InMemoryTransport.Configuration;


    public interface IInMemoryConsumeTopologyConfigurator :
        IConsumeTopologyConfigurator,
        IInMemoryConsumeTopology
    {
        new IInMemoryMessageConsumeTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;

        void AddSpecification(IInMemoryConsumeTopologySpecification specification);
    }
}
