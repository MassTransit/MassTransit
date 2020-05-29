namespace MassTransit.AmazonSqsTransport.Topology
{
    using System;
    using Builders;
    using Entities;
    using MassTransit.Topology;


    public interface IAmazonSqsMessagePublishTopology<TMessage> :
        IMessagePublishTopology<TMessage>,
        IAmazonSqsMessagePublishTopology
        where TMessage : class
    {
        Topic Topic { get; }

        /// <summary>
        /// Returns the send settings for a publish endpoint, which are mostly unused now with topology
        /// </summary>
        /// <returns></returns>
        PublishSettings GetPublishSettings(Uri hostAddress);

        BrokerTopology GetBrokerTopology(PublishBrokerTopologyOptions options = PublishBrokerTopologyOptions.MaintainHierarchy);
    }


    public interface IAmazonSqsMessagePublishTopology
    {
        /// <summary>
        /// Apply the message topology to the builder, including any implemented types
        /// </summary>
        /// <param name="builder">The topology builder</param>
        void Apply(IPublishEndpointBrokerTopologyBuilder builder);
    }
}
