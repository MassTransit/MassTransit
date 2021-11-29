namespace MassTransit
{
    using System;
    using ActiveMqTransport;
    using ActiveMqTransport.Topology;


    public interface IActiveMqMessagePublishTopology<TMessage> :
        IMessagePublishTopology<TMessage>,
        IActiveMqMessagePublishTopology
        where TMessage : class
    {
        Topic Topic { get; }

        /// <summary>
        /// Returns the send settings for a publish endpoint, which are mostly unused now with topology
        /// </summary>
        /// <param name="hostAddress"></param>
        /// <returns></returns>
        SendSettings GetSendSettings(Uri hostAddress);

        BrokerTopology GetBrokerTopology(PublishBrokerTopologyOptions options = PublishBrokerTopologyOptions.MaintainHierarchy);
    }


    public interface IActiveMqMessagePublishTopology
    {
        /// <summary>
        /// Apply the message topology to the builder, including any implemented types
        /// </summary>
        /// <param name="builder">The topology builder</param>
        void Apply(IPublishEndpointBrokerTopologyBuilder builder);
    }
}
