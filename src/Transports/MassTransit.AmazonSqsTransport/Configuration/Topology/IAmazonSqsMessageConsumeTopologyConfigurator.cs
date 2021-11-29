namespace MassTransit
{
    using System;
    using AmazonSqsTransport;
    using AmazonSqsTransport.Topology;
    using Topology;


    public interface IAmazonSqsMessageConsumeTopologyConfigurator<TMessage> :
        IMessageConsumeTopologyConfigurator<TMessage>,
        IAmazonSqsMessageConsumeTopology<TMessage>
        where TMessage : class
    {
        /// <summary>
        /// Adds the exchange bindings for this message type
        /// </summary>
        /// <param name="configure">Configure the binding and the exchange</param>
        void Subscribe(Action<IAmazonSqsTopicSubscriptionConfigurator> configure = null);
    }


    public interface IAmazonSqsMessageConsumeTopologyConfigurator :
        IMessageConsumeTopologyConfigurator
    {
        /// <summary>
        /// Apply the message topology to the builder
        /// </summary>
        /// <param name="builder"></param>
        void Apply(IReceiveEndpointBrokerTopologyBuilder builder);
    }
}
