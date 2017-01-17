namespace MassTransit.Transports.InMemory.Topology
{
    using Builders;
    using MassTransit.Topology;


    public interface IInMemoryConsumeTopology :
        IConsumeTopology
    {
        new IInMemoryMessageConsumeTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;

        /// <summary>
        /// Apply the entire topology to the builder
        /// </summary>
        /// <param name="builder"></param>
        void Apply(IInMemoryConsumeTopologyBuilder builder);
    }
}