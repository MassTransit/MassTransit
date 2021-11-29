namespace MassTransit
{
    using System;
    using RabbitMqTransport.Topology;


    public interface IRabbitMqMessageConsumeTopologyConfigurator<TMessage> :
        IMessageConsumeTopologyConfigurator<TMessage>,
        IRabbitMqMessageConsumeTopology<TMessage>
        where TMessage : class
    {
        /// <summary>
        /// Adds the exchange bindings for this message type
        /// </summary>
        /// <param name="configure">Configure the binding and the exchange</param>
        void Bind(Action<IRabbitMqExchangeBindingConfigurator> configure = null);
    }


    public interface IRabbitMqMessageConsumeTopologyConfigurator :
        IMessageConsumeTopologyConfigurator
    {
        /// <summary>
        /// Apply the message topology to the builder
        /// </summary>
        /// <param name="builder"></param>
        void Apply(IReceiveEndpointBrokerTopologyBuilder builder);
    }
}
