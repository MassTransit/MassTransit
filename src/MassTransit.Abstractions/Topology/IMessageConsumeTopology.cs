namespace MassTransit
{
    using Configuration;


    /// <summary>
    /// The message-specific Consume topology, which may be configured or otherwise
    /// setup for use with the Consume specification.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IMessageConsumeTopology<TMessage>
        where TMessage : class
    {
        void Apply(ITopologyPipeBuilder<ConsumeContext<TMessage>> builder);
    }
}
