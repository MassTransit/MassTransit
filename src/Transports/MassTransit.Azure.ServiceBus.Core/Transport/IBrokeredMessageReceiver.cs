namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Pipeline;
    using Microsoft.Azure.ServiceBus;
    using Transports;
    using Transports.Metrics;


    public interface IBrokeredMessageReceiver :
        IDispatchMetrics,
        IReceiveObserverConnector,
        IPublishObserverConnector,
        ISendObserverConnector,
        IConsumeMessageObserverConnector,
        IConsumeObserverConnector,
        IProbeSite
    {
        /// <summary>
        /// Handles the <paramref name="message"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken">Specify an optional cancellationToken</param>
        /// <param name="contextCallback">Callback to adjust the context</param>
        /// <returns></returns>
        Task Handle(Message message, CancellationToken cancellationToken = default, Action<ReceiveContext> contextCallback = null);
    }
}
