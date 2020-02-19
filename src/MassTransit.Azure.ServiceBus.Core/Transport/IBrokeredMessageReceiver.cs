namespace MassTransit.Azure.ServiceBus.Core.Transport
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Pipeline;
    using Microsoft.Azure.ServiceBus;
    using Transports;


    public interface IBrokeredMessageReceiver :
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
        Task Handle(Message message, Action<ReceiveContext> contextCallback = null);
    }
}
