namespace MassTransit
{
    using Azure.Messaging.ServiceBus.Administration;
    using AzureServiceBusTransport;
    using AzureServiceBusTransport.Configuration;
    using AzureServiceBusTransport.Topology;


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

        ServiceBusSubscriptionConfigurator GetSubscriptionConfigurator(string subscriptionName);
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
