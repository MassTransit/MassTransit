namespace MassTransit
{
    using System;


    public interface IServiceBusBusTopology :
        IBusTopology
    {
        new IServiceBusPublishTopology PublishTopology { get; }

        new IServiceBusSendTopology SendTopology { get; }

        new IServiceBusMessagePublishTopology<T> Publish<T>()
            where T : class;

        new IServiceBusMessageSendTopology<T> Send<T>()
            where T : class;

        /// <summary>
        /// Returns the destination address for the specified queue
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="configure">Callback to configure queue settings</param>
        /// <returns></returns>
        Uri GetDestinationAddress(string queueName, Action<IServiceBusQueueConfigurator> configure = null);
    }
}
