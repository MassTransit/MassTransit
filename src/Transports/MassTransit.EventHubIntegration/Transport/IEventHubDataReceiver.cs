namespace MassTransit.EventHubIntegration.Transport
{
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Processor;
    using GreenPipes;
    using Pipeline;
    using Transports;
    using Transports.Metrics;


    public interface IEventHubDataReceiver :
        IDispatchMetrics,
        IReceiveObserverConnector,
        IReceiveTransportObserverConnector,
        IPublishObserverConnector,
        ISendObserverConnector,
        IConsumeMessageObserverConnector,
        IConsumeObserverConnector,
        IProbeSite
    {
        /// <summary>
        /// Handles the <paramref name="@event" />
        /// </summary>
        /// <param name="event"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Handle(ProcessEventArgs @event, CancellationToken cancellationToken);
    }
}
