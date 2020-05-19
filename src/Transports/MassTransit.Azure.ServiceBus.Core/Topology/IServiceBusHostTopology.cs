namespace MassTransit.Azure.ServiceBus.Core.Topology
{
    using System;
    using MassTransit.Topology;


    public interface IServiceBusHostTopology :
        IHostTopology
    {
        new IServiceBusPublishTopology PublishTopology { get; }

        new IServiceBusSendTopology SendTopology { get; }

        new IServiceBusMessagePublishTopology<T> Publish<T>()
            where T : class;

        new IServiceBusMessageSendTopology<T> Send<T>()
            where T : class;

        /// <summary>
        /// Returns the destination address for the specified exchange
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="configure">Callback to configure queue settings</param>
        /// <returns></returns>
        Uri GetDestinationAddress(string queueName, Action<IQueueConfigurator> configure = null);

        /// <summary>
        /// Returns the destination address for the specified message type
        /// </summary>
        /// <param name="messageType">The message type</param>
        /// <param name="configure">Callback to configure exchange settings</param>
        /// <returns></returns>
        Uri GetDestinationAddress(Type messageType, Action<IQueueConfigurator> configure = null);
    }
}
