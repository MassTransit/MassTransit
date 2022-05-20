namespace MassTransit
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// A service endpoint has an inbound transport that pushes messages to consumers
    /// </summary>
    public interface IReceiveEndpoint :
        ISendEndpointProvider,
        IPublishEndpointProvider,
        IConsumePipeConnector,
        IRequestPipeConnector,
        IReceiveObserverConnector,
        IConsumeObserverConnector,
        IConsumeMessageObserverConnector,
        IProbeSite
    {
        Uri InputAddress { get; }

        Task<ReceiveEndpointReady> Started { get; }

        /// <summary>
        /// Start the receive endpoint
        /// </summary>
        /// <param name="cancellationToken">Cancel the start operation in progress</param>
        /// <returns>An awaitable task that is completed once everything is started</returns>
        ReceiveEndpointHandle Start(CancellationToken cancellationToken = default);

        /// <summary>
        /// Stop the receive endpoint.
        /// </summary>
        /// <param name="cancellationToken">Cancel the stop operation in progress</param>
        /// <returns>An awaitable task that is completed once everything is stopped</returns>
        Task Stop(CancellationToken cancellationToken = default);
    }
}
