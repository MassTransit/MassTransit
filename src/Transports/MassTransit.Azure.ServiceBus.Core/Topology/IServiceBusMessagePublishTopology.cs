namespace MassTransit.Azure.ServiceBus.Core.Topology
{
    using Builders;
    using Configurators;
    using MassTransit.Topology;
    using Microsoft.Azure.ServiceBus.Management;
    using Transport;


    public interface IServiceBusMessagePublishTopology<TMessage> :
        IMessagePublishTopology<TMessage>,
        IServiceBusMessagePublishTopology
        where TMessage : class
    {
        /// <summary>
        /// Returns the topic description for the message type
        /// </summary>
        TopicDescription TopicDescription { get; }

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
