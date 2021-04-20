namespace MassTransit.GrpcTransport.Topology.Configurators
{
    using Contracts;
    using GrpcTransport.Builders;
    using MassTransit.Topology;


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
        void Apply(IGrpcConsumeTopologyBuilder builder);
    }
}
