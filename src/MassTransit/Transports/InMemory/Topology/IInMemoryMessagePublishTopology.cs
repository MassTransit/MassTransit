namespace MassTransit.Transports.InMemory.Topology
{
    using Builders;
    using MassTransit.Topology;


    public interface IInMemoryMessagePublishTopology<TMessage> :
        IMessagePublishTopology<TMessage>,
        IInMemoryMessagePublishTopology
        where TMessage : class
    {
    }


    public interface IInMemoryMessagePublishTopology
    {
        /// <summary>
        /// Apply the message topology to the builder, including any implemented types
        /// </summary>
        /// <param name="builder">The topology builder</param>
        void Apply(IInMemoryPublishTopologyBuilder builder);
    }
}
