namespace MassTransit
{
    using Configuration;
    using Transports.Fabric;


    public interface IInMemoryConsumeTopology :
        IConsumeTopology
    {
        new IInMemoryMessageConsumeTopology<T> GetMessageTopology<T>()
            where T : class;

        /// <summary>
        /// Apply the entire topology to the builder
        /// </summary>
        /// <param name="builder"></param>
        void Apply(IMessageFabricConsumeTopologyBuilder builder);
    }
}
