namespace MassTransit.RabbitMqTransport.Topology
{
    using System;
    using Builders;
    using Entities;
    using MassTransit.Topology;


    public interface IRabbitMqMessagePublishTopology<TMessage> :
        IMessagePublishTopology<TMessage>,
        IRabbitMqMessagePublishTopology
        where TMessage : class
    {
        Exchange Exchange { get; }

        /// <summary>
        /// Returns the send settings for a publish endpoint, which are mostly unused now with topology
        /// </summary>
        /// <param name="hostAddress"></param>
        /// <returns></returns>
        SendSettings GetSendSettings(Uri hostAddress);

        BrokerTopology GetBrokerTopology();
    }


    public interface IRabbitMqMessagePublishTopology
    {
        /// <summary>
        /// Apply the message topology to the builder, including any implemented types
        /// </summary>
        /// <param name="builder">The topology builder</param>
        void Apply(IPublishEndpointBrokerTopologyBuilder builder);
    }
}
