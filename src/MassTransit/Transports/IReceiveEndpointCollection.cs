namespace MassTransit.Transports
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;


    public interface IReceiveEndpointCollection :
        IReceiveEndpointObserverConnector,
        IConsumeMessageObserverConnector,
        IProbeSite
    {
        /// <summary>
        /// Add an endpoint to the collection
        /// </summary>
        /// <param name="endpointName"></param>
        /// <param name="endpoint"></param>
        void Add(string endpointName, ReceiveEndpoint endpoint);

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

        /// <summary>
        /// Stop all receive endpoints
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StopEndpoints(CancellationToken cancellationToken);

        IEnumerable<EndpointHealthResult> CheckEndpointHealth();
    }
}
