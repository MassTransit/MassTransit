#nullable enable
namespace MassTransit
{
    using Configuration;
    using Transports.Fabric;


    public interface IInMemoryMessageConsumeTopologyConfigurator<TMessage> :
        IMessageConsumeTopologyConfigurator<TMessage>,
        IInMemoryMessageConsumeTopology<TMessage>
        where TMessage : class
    {
        /// <summary>
        /// Adds the exchange bindings for this message type
        /// </summary>
        void Bind(ExchangeType? exchangeType = default, string? routingKey = null);
    }


    public interface IInMemoryMessageConsumeTopologyConfigurator :
        IMessageConsumeTopologyConfigurator
    {
        /// <summary>
        /// Apply the message topology to the builder
        /// </summary>
        /// <param name="builder"></param>
        void Apply(IMessageFabricConsumeTopologyBuilder builder);
    }
}
