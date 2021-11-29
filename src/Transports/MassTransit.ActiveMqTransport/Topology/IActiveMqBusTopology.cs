namespace MassTransit
{
    using System;
    using ActiveMqTransport;
    using ActiveMqTransport.Topology;


    public interface IActiveMqBusTopology :
        IBusTopology
    {
        new IActiveMqPublishTopology PublishTopology { get; }

        new IActiveMqSendTopology SendTopology { get; }

        /// <summary>
        /// Returns the destination address for the specified exchange
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="configure">Callback to configure exchange settings</param>
        /// <returns></returns>
        Uri GetDestinationAddress(string topicName, Action<IActiveMqTopicConfigurator> configure = null);

        /// <summary>
        /// Returns the destination address for the specified message type
        /// </summary>
        /// <param name="messageType">The message type</param>
        /// <param name="configure">Callback to configure exchange settings</param>
        /// <returns></returns>
        Uri GetDestinationAddress(Type messageType, Action<IActiveMqTopicConfigurator> configure = null);

        /// <summary>
        /// Returns the settings for sending to the specified address. Will parse any arguments
        /// off the query string to properly configure the settings, including exchange and queue
        /// durability, etc.
        /// </summary>
        /// <param name="address">The ActiveMQ endpoint address</param>
        /// <returns>The send settings for the address</returns>
        SendSettings GetSendSettings(Uri address);

        new IActiveMqMessagePublishTopology<T> Publish<T>()
            where T : class;

        new IActiveMqMessageSendTopology<T> Send<T>()
            where T : class;
    }
}
