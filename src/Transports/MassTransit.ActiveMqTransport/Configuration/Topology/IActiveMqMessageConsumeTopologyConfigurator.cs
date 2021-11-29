namespace MassTransit
{
    using System;
    using ActiveMqTransport.Topology;


    public interface IActiveMqMessageConsumeTopologyConfigurator<TMessage> :
        IMessageConsumeTopologyConfigurator<TMessage>,
        IActiveMqMessageConsumeTopology<TMessage>
        where TMessage : class
    {
        /// <summary>
        /// Adds the exchange bindings for this message type
        /// </summary>
        /// <param name="configure">Configure the binding and the exchange</param>
        void Bind(Action<IActiveMqTopicBindingConfigurator> configure = null);
    }


    public interface IActiveMqMessageConsumeTopologyConfigurator :
        IMessageConsumeTopologyConfigurator
    {
        /// <summary>
        /// Apply the message topology to the builder
        /// </summary>
        /// <param name="builder"></param>
        void Apply(IReceiveEndpointBrokerTopologyBuilder builder);
    }
}
