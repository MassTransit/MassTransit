namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.ServiceBus;
    using Transports;


    public interface IServiceBusMessageReceiver :
        IDispatchMetrics,
        IReceiveObserverConnector,
        IPublishObserverConnector,
        ISendObserverConnector,
        IConsumeMessageObserverConnector,
        IConsumeObserverConnector,
        IProbeSite
    {
        /// <summary>
        /// Handles the <paramref name="message" />
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken">Specify an optional cancellationToken</param>
        /// <param name="contextCallback">Callback to adjust the context</param>
        /// <returns></returns>
        Task Handle(ServiceBusReceivedMessage message, CancellationToken cancellationToken = default, Action<ReceiveContext> contextCallback = null);
    }
}
