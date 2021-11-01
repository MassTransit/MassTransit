namespace MassTransit.Azure.ServiceBus.Core.Topology
{
    using Builders;
    using Configurators;
    using global::Azure.Messaging.ServiceBus.Administration;
    using MassTransit.Topology;
    using Transport;


    public interface IServiceBusMessagePublishTopology<TMessage> :
        IMessagePublishTopology<TMessage>,
        IServiceBusMessagePublishTopology
        where TMessage : class
    {
        /// <summary>
        /// Returns the topic options for the message type
        /// </summary>
        CreateTopicOptions CreateTopicOptions { get; }

        SendSettings GetSendSettings();

        SubscriptionConfigurator GetSubscriptionConfigurator(string subscriptionName);
    }


    public interface IServiceBusMessagePublishTopology
    {
        /// <summary>
        /// Apply the message topology to the builder, including any implemented types
        /// </summary>
        /// <param name="builder">The topology builder</param>
        void Apply(IPublishEndpointBrokerTopologyBuilder builder);
    }
}
