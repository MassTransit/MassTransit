namespace MassTransit
{
    using Configuration;


    /// <summary>
    /// The message-specific send topology, which may be configured or otherwise
    /// setup for use with the send specification.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IMessageSendTopology<TMessage>
        where TMessage : class
    {
        void Apply(ITopologyPipeBuilder<SendContext<TMessage>> builder);
    }
}
