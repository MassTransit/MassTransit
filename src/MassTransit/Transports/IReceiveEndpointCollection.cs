namespace MassTransit.Transports
{
    using System.Threading;
    using GreenPipes;
    using GreenPipes.Agents;
    using Pipeline;


    public interface IReceiveEndpointCollection :
        IReceiveObserverConnector,
        IReceiveEndpointObserverConnector,
        IConsumeMessageObserverConnector,
        IConsumeObserverConnector,
        IPublishObserverConnector,
        ISendObserverConnector,
        IAgent,
        IProbeSite
    {
        /// <summary>
        /// Add an endpoint to the collection
        /// </summary>
        /// <param name="endpointName"></param>
        /// <param name="endpoint"></param>
        void Add(string endpointName, IReceiveEndpointControl endpoint);

        /// <summary>
        /// Start all endpoints in the collection which have not been started, and return the handles
        /// for those endpoints.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        HostReceiveEndpointHandle[] StartEndpoints(CancellationToken cancellationToken);

        /// <summary>
        /// Start a new receive endpoint
        /// </summary>
        /// <param name="endpointName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        HostReceiveEndpointHandle Start(string endpointName, CancellationToken cancellationToken = default);
    }
}
