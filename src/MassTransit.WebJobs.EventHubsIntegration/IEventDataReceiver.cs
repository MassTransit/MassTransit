namespace MassTransit.WebJobs.EventHubsIntegration
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.Azure.EventHubs;
    using Pipeline;
    using Transports;


    public interface IEventDataReceiver :
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
        /// <param name="contextCallback">Callback to adjust the context</param>
        /// <returns></returns>
        Task Handle(EventData message, Action<ReceiveContext> contextCallback = null);
    }
}
