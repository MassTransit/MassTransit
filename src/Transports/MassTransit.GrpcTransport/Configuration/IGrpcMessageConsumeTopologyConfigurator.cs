namespace MassTransit
{
    using Configuration;
    using Transports.Fabric;


    public interface IGrpcMessageConsumeTopologyConfigurator<TMessage> :
        IMessageConsumeTopologyConfigurator<TMessage>,
        IGrpcMessageConsumeTopology<TMessage>
        where TMessage : class
    {
        /// <summary>
        /// Adds the exchange bindings for this message type
        /// </summary>
        void Bind(ExchangeType? exchangeType = ExchangeType.FanOut, string routingKey = default);
    }


    public interface IGrpcMessageConsumeTopologyConfigurator :
        IMessageConsumeTopologyConfigurator
    {
        /// <summary>
        /// Apply the message topology to the builder
        /// </summary>
        /// <param name="builder"></param>
        void Apply(IMessageFabricConsumeTopologyBuilder builder);
    }
}
