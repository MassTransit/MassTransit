namespace MassTransit.EventHubIntegration.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Processor;
    using Pipeline;
    using Transports;
    using Transports.Metrics;


    public interface IEventHubReceiveTransport :
        IDispatchMetrics,
        IConsumeMessageObserverConnector,
        IConsumeObserverConnector,
        IReceiveTransport
    {
        /// <summary>
        /// Handles the <paramref name="@event" />
        /// </summary>
        /// <param name="event"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="contextCallback">Callback to adjust the context</param>
        /// <returns></returns>
        Task Handle(ProcessEventArgs @event, CancellationToken cancellationToken, Action<ReceiveContext> contextCallback = null);
    }
}
