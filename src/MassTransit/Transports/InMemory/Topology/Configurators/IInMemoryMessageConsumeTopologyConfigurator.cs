namespace MassTransit.Transports.InMemory.Topology.Configurators
{
    using InMemory.Builders;
    using MassTransit.Topology;


    public interface IInMemoryMessageConsumeTopologyConfigurator<TMessage> :
        IMessageConsumeTopologyConfigurator<TMessage>,
        IInMemoryMessageConsumeTopology<TMessage>
        where TMessage : class
    {
        /// <summary>
        /// Adds the exchange bindings for this message type
        /// </summary>
        void Bind();
    }


    public interface IInMemoryMessageConsumeTopologyConfigurator :
        IMessageConsumeTopologyConfigurator
    {
        /// <summary>
        /// Apply the message topology to the builder
        /// </summary>
        /// <param name="builder"></param>
        void Apply(IInMemoryConsumeTopologyBuilder builder);
    }
}
