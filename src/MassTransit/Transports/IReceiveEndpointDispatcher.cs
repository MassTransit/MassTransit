namespace MassTransit.Transports
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Metrics;
    using Pipeline;


    public interface IReceiveEndpointDispatcher :
        IConsumeObserverConnector,
        IConsumeMessageObserverConnector,
        IDispatchMetrics,
        IReceiveObserverConnector,
        IPublishObserverConnector,
        ISendObserverConnector,
        IProbeSite
    {
        Uri InputAddress { get; }

        /// <summary>
        /// Handles the message based upon the endpoint configuration
        /// </summary>
        /// <param name="body">The message body</param>
        /// <param name="headers">The message headers</param>
        /// <param name="cancellationToken"></param>
        /// <param name="payloads">One or more payloads to add to the receive context</param>
        /// <returns></returns>
        Task Dispatch(byte[] body, IReadOnlyDictionary<string, object> headers, CancellationToken cancellationToken, params object[] payloads);
    }


    public interface IReceiveEndpointDispatcher<T> :
        IReceiveEndpointDispatcher
        where T : class
    {
    }
}
