#nullable enable
namespace MassTransit
{
    using System;


    public interface IServiceBusBusFactoryConfigurator :
        IBusFactoryConfigurator<IServiceBusReceiveEndpointConfigurator>,
        IServiceBusQueueEndpointConfigurator
    {
        new IServiceBusSendTopologyConfigurator SendTopology { get; }

        new IServiceBusPublishTopologyConfigurator PublishTopology { get; }

        /// <summary>
        /// Configure the send topology of the message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configureTopology"></param>
        void Send<T>(Action<IServiceBusMessageSendTopologyConfigurator<T>> configureTopology)
            where T : class;

        /// <summary>
        /// Configure the send topology of the message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configureTopology"></param>
        void Publish<T>(Action<IServiceBusMessagePublishTopologyConfigurator<T>> configureTopology)
            where T : class;

        /// <summary>
        /// In most cases, this is not needed and should not be used. However, if for any reason the default bus
        /// endpoint queue name needs to be changed, this will do it. Do NOT set it to the same name as a receive
        /// endpoint or you will screw things up.
        /// </summary>
        void OverrideDefaultBusEndpointQueueName(string value);

        /// <summary>
        /// Sets the namespace separator to tilde instead of slash, which is compatible with managed identities and RBAC.
        /// </summary>
        void SetNamespaceSeparatorToTilde();

        /// <summary>
        /// Sets the namespace separator to underscore instead of slash, which is compatible with managed identities and RBAC.
        /// This is automatically set when using a managed identity token provider.
        /// </summary>
        void SetNamespaceSeparatorToUnderscore();

        /// <summary>
        /// Sets the namespace separator to the specified string instead of slash.
        /// </summary>
        void SetNamespaceSeparatorTo(string separator);

        /// <summary>
        /// Configures a host
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        void Host(ServiceBusHostSettings settings);

        /// <summary>
        /// Declare a subscription endpoint on the broker and configure the endpoint settings and message consumers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subscriptionName"></param>
        /// <param name="configure"></param>
        void SubscriptionEndpoint<T>(string subscriptionName, Action<IServiceBusSubscriptionEndpointConfigurator> configure)
            where T : class;

        /// <summary>
        /// Declare a subscription endpoint on the broker and configure the endpoint settings and message consumers
        /// </summary>
        /// <param name="subscriptionName">The name of the subscription</param>
        /// <param name="topicPath">The topic name to subscribe</param>
        /// <param name="configure"></param>
        void SubscriptionEndpoint(string subscriptionName, string topicPath, Action<IServiceBusSubscriptionEndpointConfigurator> configure);
    }
}
