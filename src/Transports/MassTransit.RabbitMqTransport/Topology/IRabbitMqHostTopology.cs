namespace MassTransit.RabbitMqTransport.Topology
{
    using System;
    using MassTransit.Topology;


    public interface IRabbitMqHostTopology :
        IHostTopology
    {
        new IRabbitMqPublishTopology PublishTopology { get; }

        new IRabbitMqSendTopology SendTopology { get; }

        new IRabbitMqMessagePublishTopology<T> Publish<T>()
            where T : class;

        new IRabbitMqMessageSendTopology<T> Send<T>()
            where T : class;

        /// <summary>
        /// Returns the destination address for the specified exchange
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="configure">Callback to configure exchange settings</param>
        /// <returns></returns>
        Uri GetDestinationAddress(string exchangeName, Action<IExchangeConfigurator> configure = null);

        /// <summary>
        /// Returns the destination address for the specified message type
        /// </summary>
        /// <param name="messageType">The message type</param>
        /// <param name="configure">Callback to configure exchange settings</param>
        /// <returns></returns>
        Uri GetDestinationAddress(Type messageType, Action<IExchangeConfigurator> configure = null);

        /// <summary>
        /// Returns the address for the delayed exchanged associated with the specified <paramref name="address"/>
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        Uri GetDelayedExchangeDestinationAddress(Uri address);
    }
}
