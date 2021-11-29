namespace MassTransit
{
    using System;
    using AmazonSqsTransport;


    public interface IAmazonSqsBusTopology :
        IBusTopology
    {
        new IAmazonSqsPublishTopology PublishTopology { get; }

        new IAmazonSqsSendTopology SendTopology { get; }

        /// <summary>
        /// Returns the destination address for the specified topic
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="configure">Callback to configure exchange settings</param>
        /// <returns></returns>
        Uri GetDestinationAddress(string topicName, Action<IAmazonSqsTopicConfigurator> configure = null);

        /// <summary>
        /// Returns the destination address for the topic identified by the message type
        /// </summary>
        /// <param name="messageType">The message type</param>
        /// <param name="configure">Callback to configure exchange settings</param>
        /// <returns></returns>
        Uri GetDestinationAddress(Type messageType, Action<IAmazonSqsTopicConfigurator> configure = null);

        /// <summary>
        /// Returns the settings for sending to the specified address. Will parse any arguments
        /// off the query string to properly configure the settings, including exchange and queue
        /// durability, etc.
        /// </summary>
        /// <param name="address">The AmazonSQS endpoint address</param>
        /// <returns>The send settings for the address</returns>
        SendSettings GetSendSettings(Uri address);

        new IAmazonSqsMessagePublishTopology<T> Publish<T>()
            where T : class;

        new IAmazonSqsMessageSendTopology<T> Send<T>()
            where T : class;
    }
}
